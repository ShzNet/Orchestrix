namespace Orchestrix.Coordinator.Ownership;

/// <summary>
/// Information about job ownership by a follower node.
/// </summary>
public class JobOwnershipInfo
{
    /// <summary>
    /// Job identifier.
    /// </summary>
    public Guid JobId { get; set; }

    /// <summary>
    /// Follower node that owns this job.
    /// </summary>
    public string NodeId { get; set; } = string.Empty;

    /// <summary>
    /// When ownership was claimed.
    /// </summary>
    public DateTimeOffset ClaimedAt { get; set; }

    /// <summary>
    /// Execution ID for this job dispatch.
    /// </summary>
    public Guid ExecutionId { get; set; }
}
