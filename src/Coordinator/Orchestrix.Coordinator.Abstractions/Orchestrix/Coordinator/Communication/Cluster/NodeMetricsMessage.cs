using Orchestrix.Coordinator;

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
    /// Current status of the node.
    /// </summary>
    public NodeStatus Status { get; set; }

    /// <summary>
    /// Timestamp when metrics were collected.
    /// Also serves as heartbeat timestamp.
    /// </summary>
    public DateTimeOffset Timestamp { get; set; }

    /// <summary>
    /// Number of jobs currently active/monitored by this node.
    /// </summary>
    public int JobCount { get; set; }

    /// <summary>
    /// Number of jobs queued and waiting for distribution (System-wide or partitioned).
    /// </summary>
    public int QueuedJobCount { get; set; }

    /// <summary>
    /// CPU usage in millicores (1000m = 1 core).
    /// Compatible with Kubernetes CPU metrics.
    /// </summary>
    public long CpuMillicores { get; set; }

    /// <summary>
    /// Memory usage in bytes.
    /// </summary>
    public long MemoryUsageBytes { get; set; }

    /// <summary>
    /// Hostname of the node.
    /// </summary>
    public string? Hostname { get; set; }

    /// <summary>
    /// Process ID.
    /// </summary>
    public int? ProcessId { get; set; }

    /// <summary>
    /// Additional metadata.
    /// </summary>
    public string? Metadata { get; set; }
}
