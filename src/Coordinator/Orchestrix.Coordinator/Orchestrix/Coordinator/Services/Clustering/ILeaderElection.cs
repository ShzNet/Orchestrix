using System;

namespace Orchestrix.Coordinator.Services.Clustering;

/// <summary>
/// Manages leader election state for the coordinator node.
/// </summary>
public interface ILeaderElection
{
    /// <summary>
    /// Gets a value indicating whether this node is currently the leader.
    /// </summary>
    bool IsLeader { get; }

    /// <summary>
    /// Event raised when the leadership status changes.
    /// Payload is true if became leader, false if lost leadership.
    /// </summary>
    event EventHandler<bool>? LeadershipChanged;
}
