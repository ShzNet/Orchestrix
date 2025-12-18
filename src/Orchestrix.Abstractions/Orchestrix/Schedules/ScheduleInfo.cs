namespace Orchestrix.Schedules;

/// <summary>
/// Represents information about a recurring job schedule.
/// </summary>
public record ScheduleInfo
{
    /// <summary>
    /// Gets the unique identifier of the schedule.
    /// </summary>
    public required string Id { get; init; }

    /// <summary>
    /// Gets the type/name of the job to execute.
    /// </summary>
    public required string JobType { get; init; }

    /// <summary>
    /// Gets the queue name where jobs will be dispatched.
    /// </summary>
    public required string Queue { get; init; }

    /// <summary>
    /// Gets the cron expression for cron-based schedules.
    /// </summary>
    public string? CronExpression { get; init; }

    /// <summary>
    /// Gets the interval for interval-based schedules.
    /// </summary>
    public TimeSpan? Interval { get; init; }

    /// <summary>
    /// Gets the timestamp when the schedule will run next.
    /// </summary>
    public DateTimeOffset? NextRunAt { get; init; }

    /// <summary>
    /// Gets the timestamp when the schedule last ran.
    /// </summary>
    public DateTimeOffset? LastRunAt { get; init; }

    /// <summary>
    /// Gets a value indicating whether the schedule is enabled.
    /// </summary>
    public bool IsEnabled { get; init; }
}
