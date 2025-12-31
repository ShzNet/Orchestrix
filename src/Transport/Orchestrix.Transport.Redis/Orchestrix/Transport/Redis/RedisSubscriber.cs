using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using Orchestrix.Transport.Serialization;
using StackExchange.Redis;

namespace Orchestrix.Transport.Redis;

/// <summary>
/// Implements message subscription using Redis Streams.
/// </summary>
public class RedisSubscriber : ISubscriber
{
    private readonly IDatabase _database;
    private readonly IMessageSerializer _serializer;
    private readonly RedisTransportOptions _options;
    private readonly ILogger<RedisSubscriber> _logger;
    private readonly ConcurrentDictionary<string, CancellationTokenSource> _subscriptions = new();

    /// <summary>
    /// Initializes a new instance of <see cref="RedisSubscriber"/>.
    /// </summary>
    public RedisSubscriber(
        IConnectionMultiplexer connection,
        IMessageSerializer serializer,
        RedisTransportOptions options,
        ILogger<RedisSubscriber> logger)
    {
        _database = connection.GetDatabase();
        _serializer = serializer;
        _options = options;
        _logger = logger;
    }

    /// <inheritdoc />
    public Task SubscribeAsync<T>(string channel, Func<T, Task<bool>> handler, CancellationToken cancellationToken = default)
    {
        // For simple subscribe, use a default consumer group
        return SubscribeWithGroupAsync(channel, $"{channel}:default-group", Guid.NewGuid().ToString("N"), handler, cancellationToken);
    }

    /// <inheritdoc />
    public async Task SubscribeWithGroupAsync<T>(string stream, string groupName, string consumerName, Func<T, Task<bool>> handler, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(stream);
        ArgumentException.ThrowIfNullOrEmpty(groupName);
        ArgumentException.ThrowIfNullOrEmpty(consumerName);

        // Create consumer group if not exists
        try
        {
            await _database.StreamCreateConsumerGroupAsync(stream, groupName, StreamPosition.NewMessages, createStream: true);
            _logger.LogDebug("[RedisSubscriber] Created consumer group {Group} on stream {Stream}", groupName, stream);
        }
        catch (RedisServerException ex) when (ex.Message.Contains("BUSYGROUP"))
        {
            // Group already exists, fine
            _logger.LogDebug("[RedisSubscriber] Consumer group {Group} already exists on stream {Stream}", groupName, stream);
        }

        var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        _subscriptions[stream] = cts;

        _ = Task.Run(async () =>
        {
            _logger.LogInformation("[RedisSubscriber] Started consuming stream {Stream} as {Consumer} in group {Group}", stream, consumerName, groupName);
            
            while (!cts.Token.IsCancellationRequested)
            {
                try
                {
                    var entries = await _database.StreamReadGroupAsync(
                        stream,
                        groupName,
                        consumerName,
                        ">",  // Only new messages
                        count: 10
                    );

                    if (entries.Length == 0)
                    {
                        await Task.Delay(100, cts.Token);
                        continue;
                    }

                    foreach (var entry in entries)
                    {
                        try
                        {
                            var dataField = entry.Values.FirstOrDefault(v => v.Name == "data");
                            if (dataField.Value.IsNullOrEmpty) continue;

                            byte[]? data = dataField.Value;
                            if (data == null) continue;

                            var message = _serializer.Deserialize<T>(data);
                            if (message != null)
                            {
                                var shouldContinue = await handler(message);
                                
                                // Acknowledge the message
                                await _database.StreamAcknowledgeAsync(stream, groupName, entry.Id);
                                
                                if (!shouldContinue)
                                {
                                    _logger.LogInformation("[RedisSubscriber] Handler requested to stop consuming {Stream}", stream);
                                    return;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "[RedisSubscriber] Error handling message {Id} on stream {Stream}", entry.Id, stream);
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "[RedisSubscriber] Error reading from stream {Stream}", stream);
                    await Task.Delay(1000, cts.Token);
                }
            }
            
            _logger.LogInformation("[RedisSubscriber] Stopped consuming stream {Stream}", stream);
        }, cts.Token);

        await Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task UnsubscribeAsync(string stream)
    {
        ArgumentException.ThrowIfNullOrEmpty(stream);
        
        if (_subscriptions.TryRemove(stream, out var cts))
        {
            cts.Cancel();
            cts.Dispose();
            _logger.LogInformation("[RedisSubscriber] Unsubscribed from stream {Stream}", stream);
        }
        
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public async Task CloseChannelAsync(string stream, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(stream);
        
        // First unsubscribe
        await UnsubscribeAsync(stream);
        
        // Delete the stream (which also removes all consumer groups)
        try
        {
            var deleted = await _database.KeyDeleteAsync(stream);
            if (deleted)
            {
                _logger.LogInformation("[RedisSubscriber] Deleted stream {Stream}", stream);
            }
            else
            {
                _logger.LogDebug("[RedisSubscriber] Stream {Stream} did not exist or was already deleted", stream);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[RedisSubscriber] Failed to delete stream {Stream}", stream);
            throw;
        }
    }
}

