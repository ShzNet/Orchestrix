using Orchestrix.Coordinator.Persistence.Entities;

namespace Orchestrix.Coordinator.Scheduling;

/// <summary>
/// Evaluates schedules to determine if they are due for execution.
/// </summary>
public static class ScheduleEvaluator
{
    /// <summary>
    /// Determines if a cron schedule is due for execution.
    /// </summary>
    public static bool IsCronScheduleDue(CronScheduleEntity schedule, DateTimeOffset now)
    {
        if (!schedule.IsEnabled || schedule.NextRunTime == null)
            return false;

        return schedule.NextRunTime.Value <= now;
    }

    /// <summary>
    /// Calculates the next run time for a cron schedule.
    /// </summary>
    public static DateTimeOffset? CalculateNextCronRunTime(
        string cronExpression,
        DateTimeOffset from,
        string? timeZone = null)
    {
        var tz = string.IsNullOrEmpty(timeZone)
            ? TimeZoneInfo.Utc
            : TimeZoneInfo.FindSystemTimeZoneById(timeZone);

        return CronExpressionParser.GetNextOccurrence(cronExpression, from, tz);
    }
}
