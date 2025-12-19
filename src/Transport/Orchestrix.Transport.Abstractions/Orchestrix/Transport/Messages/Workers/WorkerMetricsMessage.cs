namespace Orchestrix.Transport.Messages.Workers;

/// <summary>
/// Worker metrics and heartbeat message.
/// Sent to worker.{workerId}.metrics channel.
/// </summary>
public class WorkerMetricsMessage
{
    /// <summary>
    /// Unique worker identifier.
    /// </summary>
    public string WorkerId { get; set; } = string.Empty;

    /// <summary>
    /// Current worker status (e.g., "Running", "Draining", "Stopped").
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// Queues this worker is subscribed to.
    /// </summary>
    public string[] Queues { get; set; } = [];

    /// <summary>
    /// Current number of jobs being processed by this worker.
    /// </summary>
    public int ActiveJobs { get; set; }

    /// <summary>
    /// Maximum concurrent jobs this worker can handle.
    /// </summary>
    public int MaxConcurrentJobs { get; set; }

    /// <summary>
    /// Timestamp when metrics were collected (also serves as heartbeat).
    /// </summary>
    public DateTimeOffset Timestamp { get; set; }

    /// <summary>
    /// CPU usage in millicores (1000m = 1 core).
    /// Compatible with Kubernetes CPU metrics.
    /// </summary>
    public long CpuMillicores { get; set; }

    /// <summary>
    /// Memory usage in bytes.
    /// </summary>
    public long MemoryUsageBytes { get; set; }
}
