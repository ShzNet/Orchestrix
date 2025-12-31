using Microsoft.Extensions.Logging;
using Orchestrix.Transport.Serialization;
using StackExchange.Redis;

namespace Orchestrix.Transport.Redis;

/// <summary>
/// Implements message publishing using Redis Streams.
/// </summary>
public class RedisPublisher : IPublisher
{
    private readonly IDatabase _database;
    private readonly IMessageSerializer _serializer;
    private readonly RedisTransportOptions _options;
    private readonly ILogger<RedisPublisher> _logger;

    /// <summary>
    /// Initializes a new instance of <see cref="RedisPublisher"/>.
    /// </summary>
    public RedisPublisher(
        IConnectionMultiplexer connection,
        IMessageSerializer serializer,
        RedisTransportOptions options,
        ILogger<RedisPublisher> logger)
    {
        _database = connection.GetDatabase();
        _serializer = serializer;
        _options = options;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task PublishAsync<T>(string channel, T message, CancellationToken cancellationToken = default)
    {
        try
        {
            var payload = _serializer.Serialize(message);
            await _database.StreamAddAsync(
                channel,
                [new NameValueEntry("data", payload)],
                maxLength: _options.MaxStreamLength,
                useApproximateMaxLength: true
            );
            _logger.LogDebug("[RedisPublisher] Published to stream {Channel}", channel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[RedisPublisher] Failed to publish to stream {Channel}", channel);
            throw;
        }
    }
}

