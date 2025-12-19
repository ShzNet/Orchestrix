using Orchestrix.Coordinator.Persistence.Entities;

namespace Orchestrix.Coordinator.Persistence;

/// <summary>
/// Repository interface for job persistence operations.
/// Focused on Coordinator-specific use cases.
/// </summary>
public interface IJobStore
{
    // === Job Lifecycle ===
    
    /// <summary>
    /// Enqueues a new job for execution.
    /// </summary>
    Task EnqueueAsync(JobEntity job, CancellationToken cancellationToken = default);

    /// <summary>
    /// Marks a job as dispatched to a worker.
    /// </summary>
    Task MarkDispatchedAsync(Guid jobId, string workerId, DateTimeOffset dispatchedAt, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates job status (Running, Completed, Failed, Cancelled).
    /// </summary>
    Task UpdateStatusAsync(Guid jobId, JobStatus status, string? error = null, DateTimeOffset? completedAt = null, CancellationToken cancellationToken = default);

    // === Scheduling & Dispatching ===
    
    /// <summary>
    /// Gets pending jobs ready for dispatch (Status = Pending, ScheduledAt &lt;= now or null).
    /// Ordered by Priority DESC, CreatedAt ASC.
    /// </summary>
    Task<IReadOnlyList<JobEntity>> GetPendingJobsAsync(int limit, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets scheduled jobs that are due for execution.
    /// </summary>
    Task<IReadOnlyList<JobEntity>> GetDueScheduledJobsAsync(DateTimeOffset now, int limit, CancellationToken cancellationToken = default);

    // === Follower Coordination ===
    
    /// <summary>
    /// Claims job ownership for a follower node.
    /// Returns true if claim succeeded, false if already claimed.
    /// </summary>
    Task<bool> TryClaimJobAsync(Guid jobId, string followerNodeId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all jobs owned by a specific follower node.
    /// Used for job handoff during scale down or crash recovery.
    /// </summary>
    Task<IReadOnlyList<JobEntity>> GetJobsByFollowerAsync(string followerNodeId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Releases all jobs owned by a dead follower node.
    /// Sets FollowerNodeId = null for reassignment.
    /// </summary>
    Task ReleaseJobsFromDeadFollowerAsync(string followerNodeId, CancellationToken cancellationToken = default);

    // === Retry & Dead Letter ===
    
    /// <summary>
    /// Increments retry count and resets job for retry.
    /// Returns true if retry is allowed, false if max retries exceeded.
    /// </summary>
    Task<bool> TryRetryJobAsync(Guid jobId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Moves a job to dead letter after max retries.
    /// </summary>
    Task MoveToDeadLetterAsync(Guid jobId, CancellationToken cancellationToken = default);

    // === Cleanup ===
    
    /// <summary>
    /// Gets completed/failed jobs that need channel cleanup.
    /// </summary>
    Task<IReadOnlyList<JobEntity>> GetJobsNeedingCleanupAsync(int limit, CancellationToken cancellationToken = default);

    /// <summary>
    /// Marks a job's channels as cleaned.
    /// </summary>
    Task MarkChannelsCleanedAsync(Guid jobId, CancellationToken cancellationToken = default);

    // === Queries (for monitoring/debugging) ===
    
    /// <summary>
    /// Gets a job by ID (for status queries).
    /// </summary>
    Task<JobEntity?> GetByIdAsync(Guid jobId, CancellationToken cancellationToken = default);
}
