namespace Orchestrix.Locking.Redis;

using StackExchange.Redis;

/// <summary>
/// Redis-based distributed lock provider.
/// </summary>
public class RedisLockProvider : IDistributedLockProvider
{
    private readonly IConnectionMultiplexer _connection;
    private readonly RedisLockOptions _options;

    public RedisLockProvider(IConnectionMultiplexer connection, RedisLockOptions options)
    {
        _connection = connection;
        _options = options;
    }

    public IDistributedLock CreateLock(string resource, DistributedLockOptions? options = null)
    {
        var database = _connection.GetDatabase();
        var key = _options.KeyPrefix + resource;
        var ttl = options?.DefaultTtl ?? TimeSpan.FromSeconds(30);
        return new RedisLock(database, key, ttl);
    }
}
