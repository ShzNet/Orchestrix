namespace Orchestrix;

/// <summary>
/// Represents the status of a job in its lifecycle.
/// </summary>
public enum JobStatus
{
    /// <summary>
    /// Job is pending and waiting to be dispatched.
    /// </summary>
    Pending,

    /// <summary>
    /// Job has been dispatched to a worker.
    /// </summary>
    Dispatched,

    /// <summary>
    /// Job is currently running on a worker.
    /// </summary>
    Running,

    /// <summary>
    /// Job completed successfully.
    /// </summary>
    Completed,

    /// <summary>
    /// Job failed (may be retried).
    /// </summary>
    Failed,

    /// <summary>
    /// Job was cancelled.
    /// </summary>
    Cancelled
}
