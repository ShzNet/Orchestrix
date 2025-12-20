namespace Orchestrix.Coordinator.Ownership;

/// <summary>
/// Registry for tracking job ownership by follower nodes.
/// </summary>
public interface IJobOwnershipRegistry
{
    /// <summary>
    /// Claims ownership of a job for this node.
    /// </summary>
    Task<bool> ClaimAsync(Guid jobId, string nodeId, Guid executionId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Releases ownership of a job.
    /// </summary>
    Task ReleaseAsync(Guid jobId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all jobs owned by this node.
    /// </summary>
    Task<IReadOnlyList<JobOwnershipInfo>> GetOwnedJobsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if this node owns the specified job.
    /// </summary>
    Task<bool> OwnsJobAsync(Guid jobId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets ownership info for a specific job.
    /// </summary>
    Task<JobOwnershipInfo?> GetOwnershipInfoAsync(Guid jobId, CancellationToken cancellationToken = default);
}
