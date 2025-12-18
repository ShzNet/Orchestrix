namespace Orchestrix.Locking.InMemory;

/// <summary>
/// In-memory distributed lock implementation using SemaphoreSlim.
/// </summary>
internal class InMemoryLock : IDistributedLock
{
    private readonly SemaphoreSlim _semaphore;
    private readonly string _resource;
    private readonly TimeSpan _defaultTtl;
    private bool _isHeld;
    private bool _disposed;

    public InMemoryLock(SemaphoreSlim semaphore, string resource, TimeSpan defaultTtl)
    {
        _semaphore = semaphore;
        _resource = resource;
        _defaultTtl = defaultTtl;
    }

    public string Resource => _resource;

    public bool IsHeld => _isHeld;

    public async Task<bool> TryAcquireAsync(TimeSpan timeout, CancellationToken ct = default)
    {
        if (_isHeld)
            return true;

        _isHeld = await _semaphore.WaitAsync(timeout, ct);
        return _isHeld;
    }

    public Task<bool> ExtendAsync(TimeSpan duration, CancellationToken ct = default)
    {
        // In-memory locks don't expire, so extension always succeeds if held
        return Task.FromResult(_isHeld);
    }

    public Task ReleaseAsync(CancellationToken ct = default)
    {
        if (_isHeld)
        {
            _semaphore.Release();
            _isHeld = false;
        }
        return Task.CompletedTask;
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
