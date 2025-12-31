using Orchestrix.Coordinator;

namespace Orchestrix.Persistence.Entities;

/// <summary>
/// Represents a coordinator node in the cluster.
/// </summary>
public class CoordinatorNodeEntity
{
    /// <summary>
    /// Unique coordinator node identifier.
    /// </summary>
    public string NodeId { get; set; } = string.Empty;

    /// <summary>
    /// Current role of this coordinator node.
    /// </summary>
    public CoordinatorRole Role { get; set; } = CoordinatorRole.Follower;

    /// <summary>
    /// Number of jobs currently owned by this node (for followers).
    /// </summary>
    public int JobCount { get; set; } = 0;

    /// <summary>
    /// Last heartbeat/metrics update timestamp.
    /// </summary>
    public DateTimeOffset LastHeartbeat { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// Node status.
    /// </summary>
    public NodeStatus Status { get; set; } = NodeStatus.Active;

    /// <summary>
    /// Additional metadata as JSON.
    /// </summary>
    public string? Metadata { get; set; }

    /// <summary>
    /// When the node first joined the cluster.
    /// </summary>
    public DateTimeOffset JoinedAt { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// Hostname or machine name where the node is running.
    /// </summary>
    public string? Hostname { get; set; }

    /// <summary>
    /// Process ID of the running node.
    /// </summary>
    public int? ProcessId { get; set; }
}


