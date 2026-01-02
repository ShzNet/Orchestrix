namespace Orchestrix.Worker;

/// <summary>
/// Represents the current state of a worker.
/// </summary>
public enum WorkerState
{
    /// <summary>
    /// Worker is starting up.
    /// </summary>
    Starting,

    /// <summary>
    /// Worker is running and accepting jobs.
    /// </summary>
    Running,

    /// <summary>
    /// Worker is draining - not accepting new jobs, cancelling running jobs.
    /// </summary>
    Draining,

    /// <summary>
    /// Worker is stopping - waiting for cancellation to propagate.
    /// </summary>
    Stopping,

    /// <summary>
    /// Worker has stopped.
    /// </summary>
    Stopped
}
