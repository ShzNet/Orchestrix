using Microsoft.Extensions.Logging;
using Orchestrix.Persistence.Entities;

namespace Orchestrix.Coordinator.Services.Scheduling;

/// <summary>
/// Implementation of schedule evaluation logic.
/// </summary>
public class ScheduleEvaluator : IScheduleEvaluator
{
    private readonly ICronExpressionParser _cronParser;
    private readonly ILogger<ScheduleEvaluator> _logger;

    /// <summary>
    /// Initializes a new instance of <see cref="ScheduleEvaluator"/>.
    /// </summary>
    /// <param name="cronParser">The cron expression parser.</param>
    /// <param name="logger">The logger.</param>
    public ScheduleEvaluator(ICronExpressionParser cronParser, ILogger<ScheduleEvaluator> logger)
    {
        _cronParser = cronParser;
        _logger = logger;
    }

    /// <inheritdoc />
    public bool IsDue(CronScheduleEntity schedule, DateTimeOffset now)
    {
        if (!schedule.IsEnabled)
            return false;

        if (schedule.NextRunTime == null)
            return false;

        return schedule.NextRunTime <= now;
    }

    /// <inheritdoc />
    public DateTimeOffset? CalculateNextRunTime(CronScheduleEntity schedule, DateTimeOffset from)
    {
        var timeZone = GetTimeZone(schedule.TimeZone);
        return _cronParser.GetNextOccurrence(schedule.CronExpression, from, timeZone);
    }

    /// <inheritdoc />
    public TimeZoneInfo GetTimeZone(string timeZone)
    {
        if (string.IsNullOrWhiteSpace(timeZone))
            return TimeZoneInfo.Utc;

        try
        {
            return TimeZoneInfo.FindSystemTimeZoneById(timeZone);
        }
        catch (TimeZoneNotFoundException)
        {
            _logger.LogWarning("TimeZone '{TimeZone}' not found, using UTC", timeZone);
            return TimeZoneInfo.Utc;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error resolving timezone '{TimeZone}', using UTC", timeZone);
            return TimeZoneInfo.Utc;
        }
    }
}
