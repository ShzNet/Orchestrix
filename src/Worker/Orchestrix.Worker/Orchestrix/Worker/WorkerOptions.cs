namespace Orchestrix.Worker;

/// <summary>
/// Configuration options for the Worker.
/// </summary>
public class WorkerOptions
{
    /// <summary>
    /// Unique identifier for this worker instance.
    /// Defaults to a random GUID if not specified.
    /// </summary>
    public string WorkerId { get; set; } = Guid.NewGuid().ToString("N");

    /// <summary>
    /// Human-readable name for this worker.
    /// Defaults to machine name.
    /// </summary>
    public string WorkerName { get; set; } = Environment.MachineName;

    /// <summary>
    /// Queue names this worker will consume jobs from.
    /// </summary>
    public string[] Queues { get; set; } = ["default"];

    /// <summary>
    /// Maximum number of concurrent jobs this worker can execute.
    /// </summary>
    public int MaxConcurrentJobs { get; set; } = 10;

    /// <summary>
    /// Interval between heartbeat/metrics broadcasts.
    /// </summary>
    public TimeSpan HeartbeatInterval { get; set; } = TimeSpan.FromSeconds(10);

    /// <summary>
    /// Default timeout for job execution if not specified per-job.
    /// </summary>
    public TimeSpan DefaultJobTimeout { get; set; } = TimeSpan.FromMinutes(5);
}
