namespace Orchestrix.Locking;

/// <summary>
/// Represents a distributed lock for coordinating access to shared resources.
/// </summary>
public interface IDistributedLock : IAsyncDisposable
{
    /// <summary>
    /// Gets the resource name being locked.
    /// </summary>
    string Resource { get; }

    /// <summary>
    /// Gets whether the lock is currently held.
    /// </summary>
    bool IsHeld { get; }

    /// <summary>
    /// Attempts to acquire the lock within the specified timeout.
    /// </summary>
    /// <param name="timeout">Maximum time to wait for lock acquisition.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>True if lock was acquired, false otherwise.</returns>
    Task<bool> TryAcquireAsync(TimeSpan timeout, CancellationToken ct = default);

    /// <summary>
    /// Extends the lock TTL by the specified duration.
    /// </summary>
    /// <param name="duration">Duration to extend the lock.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>True if extension succeeded, false if lock was lost.</returns>
    Task<bool> ExtendAsync(TimeSpan duration, CancellationToken ct = default);

    /// <summary>
    /// Releases the lock.
    /// </summary>
    /// <param name="ct">Cancellation token.</param>
    Task ReleaseAsync(CancellationToken ct = default);
}
