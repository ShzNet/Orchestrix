using Cronos;
using Orchestrix.Coordinator.Persistence;

namespace Orchestrix.Coordinator.Scheduling;

/// <summary>
/// Utility for parsing and evaluating cron expressions.
/// </summary>
public static class CronExpressionParser
{
    /// <summary>
    /// Gets the next occurrence of a cron expression from a given time.
    /// </summary>
    /// <param name="cronExpression">The cron expression (e.g., "0 0 * * *").</param>
    /// <param name="from">The time to calculate from.</param>
    /// <param name="timeZone">Optional timezone (defaults to UTC).</param>
    /// <returns>The next occurrence, or null if no future occurrence exists.</returns>
    public static DateTimeOffset? GetNextOccurrence(
        string cronExpression,
        DateTimeOffset from,
        TimeZoneInfo? timeZone = null)
    {
        try
        {
            var expression = CronExpression.Parse(cronExpression, CronFormat.IncludeSeconds);
            return expression.GetNextOccurrence(from, timeZone ?? TimeZoneInfo.Utc);
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Validates if a cron expression is valid.
    /// </summary>
    public static bool IsValid(string cronExpression)
    {
        try
        {
            CronExpression.Parse(cronExpression, CronFormat.IncludeSeconds);
            return true;
        }
        catch
        {
            return false;
        }
    }
}
