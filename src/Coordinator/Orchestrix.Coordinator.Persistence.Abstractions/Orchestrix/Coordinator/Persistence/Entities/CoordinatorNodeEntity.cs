namespace Orchestrix.Coordinator.Persistence.Entities;

/// <summary>
/// Represents a coordinator node in the cluster.
/// </summary>
public class CoordinatorNodeEntity
{
    /// <summary>
    /// Unique coordinator node identifier.
    /// </summary>
    public required string NodeId { get; init; }

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
}

/// <summary>
/// Coordinator role enumeration.
/// </summary>
public enum CoordinatorRole
{
    /// <summary>
    /// Leader coordinator (handles scheduling and dispatching).
    /// </summary>
    Leader,

    /// <summary>
    /// Follower coordinator (handles job event processing).
    /// </summary>
    Follower
}

/// <summary>
/// Coordinator node status enumeration.
/// </summary>
public enum NodeStatus
{
    /// <summary>
    /// Node is active and healthy.
    /// </summary>
    Active,

    /// <summary>
    /// Node is draining (finishing current work, not accepting new jobs).
    /// </summary>
    Draining,

    /// <summary>
    /// Node is offline/dead.
    /// </summary>
    Offline
}
