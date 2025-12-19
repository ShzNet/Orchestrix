using Orchestrix.Coordinator.Persistence.Entities;

namespace Orchestrix.Coordinator.Persistence;

/// <summary>
/// Repository interface for worker node operations.
/// Focused on worker lifecycle and capacity management.
/// </summary>
public interface IWorkerStore
{
    /// <summary>
    /// Registers or updates a worker's heartbeat and capacity info.
    /// </summary>
    Task UpdateHeartbeatAsync(WorkerEntity worker, CancellationToken cancellationToken = default);

    /// <summary>
    /// Marks a worker as draining (finishing current jobs, not accepting new ones).
    /// </summary>
    Task MarkDrainingAsync(string workerId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes a worker from the registry.
    /// </summary>
    Task RemoveWorkerAsync(string workerId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets workers that can process a specific queue, ordered by available capacity.
    /// Only returns Active workers with capacity (CurrentLoad &lt; MaxConcurrency).
    /// </summary>
    Task<IReadOnlyList<WorkerEntity>> GetAvailableWorkersForQueueAsync(string queue, CancellationToken cancellationToken = default);

    /// <summary>
    /// Detects dead workers (LastHeartbeat older than timeout threshold).
    /// </summary>
    Task<IReadOnlyList<WorkerEntity>> GetDeadWorkersAsync(TimeSpan timeout, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all active workers (for monitoring).
    /// </summary>
    Task<IReadOnlyList<WorkerEntity>> GetAllActiveWorkersAsync(CancellationToken cancellationToken = default);
}
