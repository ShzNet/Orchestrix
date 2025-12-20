namespace Orchestrix.Coordinator.Caching;

/// <summary>
/// Interface for cache invalidation operations.
/// Both Coordinator and Control Panel can use this to invalidate stale cache entries.
/// </summary>
public interface ICacheInvalidator
{
    /// <summary>
    /// Invalidates cache for a specific job.
    /// </summary>
    Task InvalidateJobAsync(Guid jobId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Invalidates cache for jobs in a specific queue.
    /// </summary>
    Task InvalidateJobsByQueueAsync(string queue, CancellationToken cancellationToken = default);

    /// <summary>
    /// Invalidates cache for jobs with a specific status.
    /// </summary>
    Task InvalidateJobsByStatusAsync(string status, CancellationToken cancellationToken = default);

    /// <summary>
    /// Invalidates cache for a specific schedule.
    /// </summary>
    Task InvalidateScheduleAsync(Guid scheduleId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Invalidates cache for all schedules.
    /// </summary>
    Task InvalidateAllSchedulesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Invalidates cache for a specific worker.
    /// </summary>
    Task InvalidateWorkerAsync(string workerId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Invalidates cache for all workers.
    /// </summary>
    Task InvalidateAllWorkersAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Invalidates cache for a specific coordinator node.
    /// </summary>
    Task InvalidateCoordinatorNodeAsync(string nodeId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Invalidates cache for all coordinator nodes.
    /// </summary>
    Task InvalidateAllCoordinatorNodesAsync(CancellationToken cancellationToken = default);
}
