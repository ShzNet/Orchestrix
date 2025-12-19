namespace Orchestrix.Coordinator.Communication.Cluster;

/// <summary>
/// Coordinator node metrics and heartbeat message.
/// Contains node-specific metrics only (not system-wide aggregates).
/// </summary>
public class NodeMetricsMessage
{
    /// <summary>
    /// Unique coordinator node identifier.
    /// </summary>
    public string NodeId { get; set; } = string.Empty;

    /// <summary>
    /// Current role of this node (Leader/Follower).
    /// </summary>
    public CoordinatorRole Role { get; set; }

    /// <summary>
    /// Timestamp when metrics were collected.
    /// Also serves as heartbeat timestamp.
    /// </summary>
    public DateTimeOffset Timestamp { get; set; }

    /// <summary>
    /// Number of jobs currently owned/followed by this node (for followers).
    /// </summary>
    public int JobCount { get; set; }

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
