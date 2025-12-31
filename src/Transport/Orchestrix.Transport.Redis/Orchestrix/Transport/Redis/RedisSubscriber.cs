using Microsoft.Extensions.Logging;
using Orchestrix.Transport.Serialization;
using StackExchange.Redis;

namespace Orchestrix.Transport.Redis;

/// <summary>
/// Implements message subscription using Redis Pub/Sub.
/// </summary>
public class RedisSubscriber(
    IConnectionMultiplexer connection,
    IMessageSerializer serializer,
    ILogger<RedisSubscriber> logger)
    : ISubscriber
{
    private readonly StackExchange.Redis.ISubscriber _subscriber = connection.GetSubscriber();

    /// <inheritdoc />
    public async Task SubscribeAsync<T>(string channel, Func<T, Task<bool>> handler, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(channel);
        await _subscriber.SubscribeAsync(RedisChannel.Literal(channel), (RedisChannel ch, RedisValue val) =>
        {
            // Fire and forget, or block? Blocking is bad. Fire and forget with error handling.
            _ = Task.Run(async () => 
            {
                try
                {
                    byte[]? data = val;
                    if (data == null) return;

                    var message = serializer.Deserialize<T>(data);
                    if (message != null)
                    {
                        await handler(message);
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "[RedisSubscriber] Error handling message on {Channel}", channel);
                }
            });
        });
        
        logger.LogInformation("[RedisSubscriber] Subscribed to {Channel}", channel);
    }

    /// <inheritdoc />
    public Task SubscribeWithGroupAsync<T>(string channel, string groupName, string consumerName, Func<T, Task<bool>> handler, CancellationToken cancellationToken = default)
    {
        // TODO: Implement Redis Streams for Consumer Groups
        logger.LogWarning("[RedisSubscriber] SubscribeWithGroupAsync is not yet implemented for Redis Pub/Sub. Falling back to simple SubscribeAsync.");
        return SubscribeAsync(channel, handler, cancellationToken);
    }

    /// <inheritdoc />
    public async Task UnsubscribeAsync(string channel)
    {
        ArgumentException.ThrowIfNullOrEmpty(channel);
        await _subscriber.UnsubscribeAsync(RedisChannel.Literal(channel));
        logger.LogInformation("[RedisSubscriber] Unsubscribed from {Channel}", channel);
    }

    /// <inheritdoc />
    public Task CloseChannelAsync(string channel, CancellationToken cancellationToken = default)
    {
        return UnsubscribeAsync(channel);
    }
}
