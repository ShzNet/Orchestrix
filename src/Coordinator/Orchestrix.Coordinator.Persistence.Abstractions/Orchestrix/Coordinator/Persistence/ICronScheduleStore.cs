using Orchestrix.Coordinator.Persistence.Entities;

namespace Orchestrix.Coordinator.Persistence;

/// <summary>
/// Repository interface for cron schedule operations.
/// Focused on scheduling use cases.
/// </summary>
public interface ICronScheduleStore
{
    /// <summary>
    /// Registers a new cron schedule.
    /// </summary>
    Task RegisterScheduleAsync(CronScheduleEntity schedule, CancellationToken cancellationToken = default);

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
    Task<IReadOnlyList<CronScheduleEntity>> GetDueSchedulesAsync(DateTimeOffset now, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes a schedule.
    /// </summary>
    Task RemoveScheduleAsync(Guid scheduleId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all schedules (for admin UI).
    /// </summary>
    Task<IReadOnlyList<CronScheduleEntity>> GetAllSchedulesAsync(CancellationToken cancellationToken = default);
}
