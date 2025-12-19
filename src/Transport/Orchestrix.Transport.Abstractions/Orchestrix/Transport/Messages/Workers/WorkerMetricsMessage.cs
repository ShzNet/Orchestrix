namespace Orchestrix.Transport.Messages.Workers;

/// <summary>
/// Worker metrics and heartbeat message.
/// Contains worker-specific metrics only (not system-wide aggregates).
/// </summary>
public class WorkerMetricsMessage
{
    /// <summary>
    /// Unique worker identifier.
    /// </summary>
    public string WorkerId { get; set; } = string.Empty;

    /// <summary>
    /// Timestamp when metrics were collected.
    /// Also serves as heartbeat timestamp.
    /// </summary>
    public DateTimeOffset Timestamp { get; set; }

    /// <summary>
    /// Current number of jobs being processed by this worker.
    /// </summary>
    public int ActiveJobs { get; set; }

    /// <summary>
    /// Maximum concurrent jobs this worker can handle.
    /// </summary>
    public int MaxConcurrency { get; set; }

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
