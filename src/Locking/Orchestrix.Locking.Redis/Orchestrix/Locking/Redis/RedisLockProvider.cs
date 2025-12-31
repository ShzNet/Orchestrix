namespace Orchestrix.Locking.Redis;

using StackExchange.Redis;

/// <summary>
/// Redis-based distributed lock provider.
/// </summary>
public class RedisLockProvider(IConnectionMultiplexer connection, RedisLockOptions options)
    : IDistributedLockProvider
{
    /// <inheritdoc/>
    public IDistributedLock CreateLock(string resource, DistributedLockOptions? options1 = null)
    {
        var database = connection.GetDatabase();
        var key = options.KeyPrefix + resource;
        var ttl = options1?.DefaultTtl ?? TimeSpan.FromSeconds(30);
        return new RedisLock(database, key, ttl);
    }
}