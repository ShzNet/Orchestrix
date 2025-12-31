using Microsoft.Extensions.Logging;
using Orchestrix.Transport.Serialization;
using StackExchange.Redis;

namespace Orchestrix.Transport.Redis;

/// <summary>
/// Implements message publishing using Redis Pub/Sub.
/// </summary>
public class RedisPublisher(
    IConnectionMultiplexer connection,
    IMessageSerializer serializer,
    ILogger<RedisPublisher> logger)
    : IPublisher
{
    private readonly IDatabase _database = connection.GetDatabase();
    private readonly StackExchange.Redis.ISubscriber _subscriber = connection.GetSubscriber();

    /// <inheritdoc />
    public async Task PublishAsync<T>(string channel, T message, CancellationToken cancellationToken = default)
    {
        try
        {
            var payload = serializer.Serialize(message);
            await _subscriber.PublishAsync(RedisChannel.Literal(channel), payload);
            logger.LogDebug("[RedisPublisher] Published to {Channel}: {Message}", channel, message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "[RedisPublisher] Failed to publish to {Channel}", channel);
            throw;
        }
    }
}
