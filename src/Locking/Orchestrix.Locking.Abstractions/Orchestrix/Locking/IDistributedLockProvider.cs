namespace Orchestrix.Locking;

/// <summary>
/// Factory for creating distributed locks.
/// </summary>
public interface IDistributedLockProvider
{
    /// <summary>
    /// Creates a distributed lock for the specified resource.
    /// </summary>
    /// <param name="resource">The resource name to lock.</param>
    /// <param name="options">Optional lock configuration.</param>
    /// <returns>A distributed lock instance.</returns>
    IDistributedLock CreateLock(string resource, DistributedLockOptions? options = null);
}
