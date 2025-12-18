namespace Orchestrix.Schedules;

/// <summary>
/// Represents information about a scheduled job.
/// </summary>
public class ScheduleInfo
{
    public string Id { get; set; } = string.Empty;
    public string JobType { get; set; } = string.Empty;
    public string Queue { get; set; } = string.Empty;
    public string? CronExpression { get; set; }
    public TimeSpan? Interval { get; set; }
    public DateTimeOffset? NextRunAt { get; set; }
    public DateTimeOffset? LastRunAt { get; set; }
    public bool IsEnabled { get; set; }
}
