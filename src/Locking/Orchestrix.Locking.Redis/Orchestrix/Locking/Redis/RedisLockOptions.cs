namespace Orchestrix.Locking.Redis;

/// <summary>
/// Configuration options for Redis-based distributed locking.
/// </summary>
public class RedisLockOptions
{
    /// <summary>
    /// Gets or sets the Redis connection string.
    /// Default is "localhost:6379".
    /// </summary>
    public string ConnectionString { get; set; } = "localhost:6379";

    /// <summary>
    /// Gets or sets the key prefix for lock keys in Redis.
    /// Default is "orchestrix:lock:".
    /// </summary>
    public string KeyPrefix { get; set; } = "orchestrix:lock:";
}
