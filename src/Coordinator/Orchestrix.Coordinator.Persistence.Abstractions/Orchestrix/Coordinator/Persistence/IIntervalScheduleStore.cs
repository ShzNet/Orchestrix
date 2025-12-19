using Orchestrix.Coordinator.Persistence.Entities;

namespace Orchestrix.Coordinator.Persistence;

/// <summary>
/// Repository interface for interval schedule operations.
/// Focused on scheduling use cases.
/// </summary>
public interface IIntervalScheduleStore
{
    /// <summary>
    /// Registers a new interval schedule.
    /// </summary>
    Task RegisterScheduleAsync(IntervalScheduleEntity schedule, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates schedule's next run time after execution.
    /// </summary>
    Task UpdateNextRunTimeAsync(Guid scheduleId, DateTimeOffset nextRunTime, DateTimeOffset lastRunTime, CancellationToken cancellationToken = default);

    /// <summary>
    /// Enables or disables a schedule.
    /// </summary>
    Task SetEnabledAsync(Guid scheduleId, bool enabled, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all schedules that are due for execution (NextRunTime &lt;= now AND IsEnabled = true).
    /// </summary>
    Task<IReadOnlyList<IntervalScheduleEntity>> GetDueSchedulesAsync(DateTimeOffset now, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes a schedule.
    /// </summary>
    Task RemoveScheduleAsync(Guid scheduleId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all schedules (for admin UI).
    /// </summary>
    Task<IReadOnlyList<IntervalScheduleEntity>> GetAllSchedulesAsync(CancellationToken cancellationToken = default);
}
