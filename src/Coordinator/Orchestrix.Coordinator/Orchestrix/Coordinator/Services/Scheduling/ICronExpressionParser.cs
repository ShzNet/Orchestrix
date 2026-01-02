using Cronos;

namespace Orchestrix.Coordinator.Services.Scheduling;

/// <summary>
/// Interface for parsing and evaluating cron expressions.
/// </summary>
public interface ICronExpressionParser
{
    /// <summary>
    /// Validates a cron expression format.
    /// </summary>
    /// <param name="cronExpression">The cron expression to validate.</param>
    /// <returns>True if valid, false otherwise.</returns>
    bool IsValid(string cronExpression);

    /// <summary>
    /// Calculates the next occurrence of a cron expression.
    /// </summary>
    /// <param name="cronExpression">The cron expression.</param>
    /// <param name="from">The start time to calculate from.</param>
    /// <param name="timeZone">The timezone for evaluation.</param>
    /// <returns>The next occurrence, or null if none.</returns>
    DateTimeOffset? GetNextOccurrence(string cronExpression, DateTimeOffset from, TimeZoneInfo? timeZone = null);
}
