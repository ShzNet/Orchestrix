namespace Orchestrix.Locking.Redis;

using StackExchange.Redis;

/// <summary>
/// Redis-based distributed lock implementation using SET NX EX pattern.
/// </summary>
internal class RedisLock(IDatabase database, string key, TimeSpan defaultTtl) : IDistributedLock
{
    private readonly string _token = Guid.NewGuid().ToString("N");
    private bool _disposed;

    // Lua script to safely release lock only if token matches
    private const string ReleaseLuaScript = """

                                                    if redis.call('get', KEYS[1]) == ARGV[1] then
                                                        return redis.call('del', KEYS[1])
                                                    else
                                                        return 0
                                                    end
                                            """;

    public string Resource => key;

    public bool IsHeld { get; private set; }

    public async Task<bool> TryAcquireAsync(TimeSpan timeout, CancellationToken ct = default)
    {
        if (IsHeld)
            return true;

        var deadline = DateTime.UtcNow.Add(timeout);
        
        while (DateTime.UtcNow < deadline)
        {
            // SET key token NX EX ttl - atomic set-if-not-exists with expiration
            IsHeld = await database.StringSetAsync(
                key,
                _token,
                defaultTtl,
                When.NotExists,
                CommandFlags.None
            );

            if (IsHeld)
                return true;

            // Wait a bit before retrying
            await Task.Delay(100, ct);
        }

        return false;
    }

    public async Task<bool> ExtendAsync(TimeSpan duration, CancellationToken ct = default)
    {
        if (!IsHeld)
            return false;

        // SET key token XX EX duration - extend only if key exists and token matches
        var result = await database.StringSetAsync(
            key,
            _token,
            duration,
            When.Exists,
            CommandFlags.None
        );

        // Verify the token still matches (could have been taken by someone else)
        if (result)
        {
            var currentToken = await database.StringGetAsync(key);
            IsHeld = currentToken == _token;
            return IsHeld;
        }

        IsHeld = false;
        return false;
    }

    public async Task ReleaseAsync(CancellationToken ct = default)
    {
        if (!IsHeld)
            return;

        try
        {
            // Use Lua script to ensure we only delete if token matches
            await database.ScriptEvaluateAsync(
                ReleaseLuaScript,
                new RedisKey[] { key },
                new RedisValue[] { _token }
            );
        }
        finally
        {
            IsHeld = false;
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (!_disposed)
        {
            await ReleaseAsync();
            _disposed = true;
        }
    }
}
