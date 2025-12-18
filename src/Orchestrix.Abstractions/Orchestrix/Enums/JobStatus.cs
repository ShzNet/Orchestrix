namespace Orchestrix.Enums;

/// <summary>
/// Represents the lifecycle status of a job.
/// </summary>
public enum JobStatus
{
    /// <summary>
    /// Job has been created but not yet queued.
    /// </summary>
    Created = 0,

    /// <summary>
    /// Job is queued and waiting to be scheduled.
    /// </summary>
    Pending = 1,

    /// <summary>
    /// Job is scheduled to run at a specific time (cron/interval).
    /// </summary>
    Scheduled = 2,

    /// <summary>
    /// Job has been dispatched to a worker queue.
    /// </summary>
    Dispatched = 3,

    /// <summary>
    /// Job is currently being executed by a worker.
    /// </summary>
    Running = 4,

    /// <summary>
    /// Job execution completed successfully.
    /// </summary>
    Completed = 5,

    /// <summary>
    /// Job execution failed (may be retried).
    /// </summary>
    Failed = 6,

    /// <summary>
    /// Job execution exceeded timeout threshold.
    /// </summary>
    TimedOut = 7,

    /// <summary>
    /// Job was cancelled by user or system.
    /// </summary>
    Cancelled = 8
}
