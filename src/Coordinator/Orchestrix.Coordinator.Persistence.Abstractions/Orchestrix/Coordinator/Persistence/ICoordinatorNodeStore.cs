using Orchestrix.Coordinator.Persistence.Entities;

namespace Orchestrix.Coordinator.Persistence;

/// <summary>
/// Repository interface for coordinator node operations.
/// Focused on cluster coordination and health monitoring.
/// </summary>
public interface ICoordinatorNodeStore
{
    /// <summary>
    /// Registers or updates a coordinator node's heartbeat and metrics.
    /// </summary>
    Task UpdateHeartbeatAsync(CoordinatorNodeEntity node, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates a node's role (Leader/Follower).
    /// </summary>
    Task UpdateRoleAsync(string nodeId, CoordinatorRole role, CancellationToken cancellationToken = default);

    /// <summary>
    /// Increments job count for a follower node.
    /// </summary>
    Task IncrementJobCountAsync(string nodeId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Decrements job count for a follower node.
    /// </summary>
    Task DecrementJobCountAsync(string nodeId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Marks a node as draining (finishing current work, not accepting new jobs).
    /// </summary>
    Task MarkDrainingAsync(string nodeId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes a node from the cluster.
    /// </summary>
    Task RemoveNodeAsync(string nodeId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Detects dead coordinator nodes (LastHeartbeat older than timeout threshold).
    /// </summary>
    Task<IReadOnlyList<CoordinatorNodeEntity>> GetDeadNodesAsync(TimeSpan timeout, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all active follower nodes, ordered by job count (for load balancing).
    /// </summary>
    Task<IReadOnlyList<CoordinatorNodeEntity>> GetActiveFollowersAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all active nodes (for monitoring).
    /// </summary>
    Task<IReadOnlyList<CoordinatorNodeEntity>> GetAllActiveNodesAsync(CancellationToken cancellationToken = default);
}
