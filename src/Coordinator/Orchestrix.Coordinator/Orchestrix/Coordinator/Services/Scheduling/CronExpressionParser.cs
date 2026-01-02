using Cronos;
using Microsoft.Extensions.Logging;

namespace Orchestrix.Coordinator.Services.Scheduling;

/// <summary>
/// Implementation of cron expression parsing using Cronos library.
/// </summary>
public class CronExpressionParser : ICronExpressionParser
{
    private readonly ILogger<CronExpressionParser> _logger;

    /// <summary>
    /// Initializes a new instance of <see cref="CronExpressionParser"/>.
    /// </summary>
    /// <param name="logger">The logger.</param>
    public CronExpressionParser(ILogger<CronExpressionParser> logger)
    {
        _logger = logger;
    }

    /// <inheritdoc />
    public bool IsValid(string cronExpression)
    {
        if (string.IsNullOrWhiteSpace(cronExpression))
            return false;

        try
        {
            // Try parsing with seconds support first, then without
            CronExpression.Parse(cronExpression, CronFormat.IncludeSeconds);
            return true;
        }
        catch (CronFormatException)
        {
            try
            {
                CronExpression.Parse(cronExpression, CronFormat.Standard);
                return true;
            }
            catch (CronFormatException)
            {
                return false;
            }
        }
    }

    /// <inheritdoc />
    public DateTimeOffset? GetNextOccurrence(string cronExpression, DateTimeOffset from, TimeZoneInfo? timeZone = null)
    {
        if (string.IsNullOrWhiteSpace(cronExpression))
        {
            _logger.LogWarning("Empty cron expression provided");
            return null;
        }

        try
        {
            // Determine cron format (with or without seconds)
            var format = cronExpression.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length > 5
                ? CronFormat.IncludeSeconds
                : CronFormat.Standard;

            var cron = CronExpression.Parse(cronExpression, format);
            var tz = timeZone ?? TimeZoneInfo.Utc;

            return cron.GetNextOccurrence(from, tz);
        }
        catch (CronFormatException ex)
        {
            _logger.LogError(ex, "Invalid cron expression: {Expression}", cronExpression);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calculating next occurrence for: {Expression}", cronExpression);
            return null;
        }
    }
}
