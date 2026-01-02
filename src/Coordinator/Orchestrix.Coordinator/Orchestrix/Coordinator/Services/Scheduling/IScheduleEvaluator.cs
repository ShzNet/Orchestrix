using Orchestrix.Persistence.Entities;

namespace Orchestrix.Coordinator.Services.Scheduling;

/// <summary>
/// Interface for schedule evaluation logic.
/// </summary>
public interface IScheduleEvaluator
{
    /// <summary>
    /// Checks if a cron schedule is due for execution.
    /// </summary>
    /// <param name="schedule">The schedule to check.</param>
    /// <param name="now">The current time.</param>
    /// <returns>True if the schedule is due.</returns>
    bool IsDue(CronScheduleEntity schedule, DateTimeOffset now);

    /// <summary>
    /// Calculates the next run time for a schedule.
    /// </summary>
    /// <param name="schedule">The schedule.</param>
    /// <param name="from">The time to calculate from (typically now).</param>
    /// <returns>The next run time, or null if invalid.</returns>
    DateTimeOffset? CalculateNextRunTime(CronScheduleEntity schedule, DateTimeOffset from);

    /// <summary>
    /// Gets the TimeZoneInfo for a schedule's timezone string.
    /// </summary>
    /// <param name="timeZone">The timezone string (e.g., "UTC", "America/New_York").</param>
    /// <returns>The TimeZoneInfo, or UTC if invalid.</returns>
    TimeZoneInfo GetTimeZone(string timeZone);
}
