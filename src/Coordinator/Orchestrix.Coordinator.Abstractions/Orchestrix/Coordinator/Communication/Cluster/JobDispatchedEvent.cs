namespace Orchestrix.Coordinator.Communication.Cluster;

/// <summary>
/// Event published when a job is dispatched to workers.
/// Broadcast to all followers for race-to-claim ownership pattern.
/// </summary>
public class JobDispatchedEvent
{
    /// <summary>
    /// Unique job identifier.
    /// </summary>
    public Guid JobId { get; set; }

    /// <summary>
    /// Execution/history ID for this dispatch attempt.
    /// Followers use this to subscribe to job.{executionId}.status and job.{executionId}.logs.
    /// </summary>
    public Guid ExecutionId { get; set; }

    /// <summary>
    /// Queue the job was dispatched to.
    /// </summary>
public string Queue { get; set; } = string.Empty;

    /// <summary>
    /// Fully qualified job type name.
    /// </summary>
    public string JobType { get; set; } = string.Empty;

    /// <summary>
    /// When the job was dispatched.
    /// </summary>
    public DateTimeOffset Timestamp { get; set; }
}
