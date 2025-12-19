namespace Orchestrix.Coordinator.Communication.Cluster;

/// <summary>
/// Request to handoff job ownership during scale down or crash recovery.
/// </summary>
public class JobHandoffMessage
{
    /// <summary>
    /// Job identifier to handoff.
    /// </summary>
    public Guid JobId { get; set; }

    /// <summary>
    /// Execution/history ID for the current job execution.
    /// New owner uses this to subscribe to job.{executionId}.status and job.{executionId}.logs.
    /// </summary>
    public Guid ExecutionId { get; set; }

    /// <summary>
    /// Node ID that is handing off the job.
    /// </summary>
    public string FromNodeId { get; set; } = string.Empty;

    /// <summary>
    /// Reason for handoff (Drain/Crash).
    /// </summary>
    public HandoffReason Reason { get; set; }

    /// <summary>
    /// When the handoff was initiated.
    /// </summary>
    public DateTimeOffset Timestamp { get; set; }
}

/// <summary>
/// Reason for job handoff.
/// </summary>
public enum HandoffReason
{
    /// <summary>
    /// Node is draining gracefully.
    /// </summary>
    Drain,

    /// <summary>
    /// Node crashed or became unresponsive.
    /// </summary>
    Crash
}
