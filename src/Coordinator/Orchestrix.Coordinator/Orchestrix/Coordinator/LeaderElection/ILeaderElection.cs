namespace Orchestrix.Coordinator.LeaderElection;

/// <summary>
/// Service for distributed leader election in Coordinator cluster.
/// </summary>
public interface ILeaderElection
{
    /// <summary>
    /// Gets whether this node is currently the leader.
    /// </summary>
    bool IsLeader { get; }

    /// <summary>
    /// Event raised when leadership status changes.
    /// </summary>
    event EventHandler<bool>? LeadershipChanged;

    /// <summary>
    /// Starts the leader election process.
    /// </summary>
    Task StartAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Stops the leader election process.
    /// </summary>
    Task StopAsync(CancellationToken cancellationToken = default);
}
