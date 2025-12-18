namespace Orchestrix.Locking.Redis;

using StackExchange.Redis;

/// <summary>
/// Redis-based distributed lock implementation using SET NX EX pattern.
/// </summary>
internal class RedisLock : IDistributedLock
{
    private readonly IDatabase _database;
    private readonly string _key;
    private readonly string _token;
    private readonly TimeSpan _defaultTtl;
    private bool _isHeld;
    private bool _disposed;

    // Lua script to safely release lock only if token matches
    private const string ReleaseLuaScript = @"
        if redis.call('get', KEYS[1]) == ARGV[1] then
            return redis.call('del', KEYS[1])
        else
            return 0
        end";

    public RedisLock(IDatabase database, string key, TimeSpan defaultTtl)
    {
        _database = database;
        _key = key;
        _token = Guid.NewGuid().ToString("N");
        _defaultTtl = defaultTtl;
    }

    public string Resource => _key;

    public bool IsHeld => _isHeld;

    public async Task<bool> TryAcquireAsync(TimeSpan timeout, CancellationToken ct = default)
    {
        if (_isHeld)
            return true;

        var deadline = DateTime.UtcNow.Add(timeout);
        
        while (DateTime.UtcNow < deadline)
        {
            // SET key token NX EX ttl - atomic set-if-not-exists with expiration
            _isHeld = await _database.StringSetAsync(
                _key,
                _token,
                _defaultTtl,
                When.NotExists,
                CommandFlags.None
            );

            if (_isHeld)
                return true;

            // Wait a bit before retrying
            await Task.Delay(100, ct);
        }

        return false;
    }

    public async Task<bool> ExtendAsync(TimeSpan duration, CancellationToken ct = default)
    {
        if (!_isHeld)
            return false;

        // SET key token XX EX duration - extend only if key exists and token matches
        var result = await _database.StringSetAsync(
            _key,
            _token,
            duration,
            When.Exists,
            CommandFlags.None
        );

        // Verify the token still matches (could have been taken by someone else)
        if (result)
        {
            var currentToken = await _database.StringGetAsync(_key);
            _isHeld = currentToken == _token;
            return _isHeld;
        }

        _isHeld = false;
        return false;
    }

    public async Task ReleaseAsync(CancellationToken ct = default)
    {
        if (!_isHeld)
            return;

        try
        {
            // Use Lua script to ensure we only delete if token matches
            await _database.ScriptEvaluateAsync(
                ReleaseLuaScript,
                new RedisKey[] { _key },
                new RedisValue[] { _token }
            );
        }
        finally
        {
            _isHeld = false;
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
