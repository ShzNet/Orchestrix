namespace Orchestrix.Schedules;

/// <summary>
/// Represents information about a scheduled job.
/// </summary>
public class ScheduleInfo
{
    /// <summary>
    /// Gets or sets the unique identifier of the schedule.
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the job type associated with the schedule.
    /// </summary>
    public string JobType { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the queue name for the scheduled job.
    /// </summary>
    public string Queue { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the cron expression defining the schedule.
    /// </summary>
    public string? CronExpression { get; set; }

    /// <summary>
    /// Gets or sets the fixed interval for the schedule, if applicable.
    /// </summary>
    public TimeSpan? Interval { get; set; }

    /// <summary>
    /// Gets or sets the timestamp for the next scheduled run.
    /// </summary>
    public DateTimeOffset? NextRunAt { get; set; }

    /// <summary>
    /// Gets or sets the timestamp when the schedule was last run.
    /// </summary>
    public DateTimeOffset? LastRunAt { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the schedule is enabled.
    /// </summary>
    public bool IsEnabled { get; set; }
}
