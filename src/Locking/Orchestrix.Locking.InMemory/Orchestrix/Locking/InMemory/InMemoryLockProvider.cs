namespace Orchestrix.Locking.InMemory;

using System.Collections.Concurrent;

/// <summary>
/// In-memory distributed lock provider.
/// </summary>
public class InMemoryLockProvider : IDistributedLockProvider
{
    private readonly ConcurrentDictionary<string, SemaphoreSlim> _semaphores = new();

    public IDistributedLock CreateLock(string resource, DistributedLockOptions? options = null)
    {
        var semaphore = _semaphores.GetOrAdd(resource, _ => new SemaphoreSlim(1, 1));
        var ttl = options?.DefaultTtl ?? TimeSpan.FromSeconds(30);
        return new InMemoryLock(semaphore, resource, ttl);
    }
}
