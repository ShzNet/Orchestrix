namespace Orchestrix.Coordinator.Persistence.Entities;

/// <summary>
/// Represents an interval-based recurring schedule.
/// </summary>
public class IntervalScheduleEntity
{
    /// <summary>
    /// Unique schedule identifier.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Human-readable schedule name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Interval between executions.
    /// </summary>
    public TimeSpan Interval { get; set; }

    /// <summary>
    /// Job type to execute.
    /// </summary>
    public string JobType { get; set; } = string.Empty;

    /// <summary>
    /// Queue for job execution.
    /// </summary>
    public string Queue { get; set; } = string.Empty;

    /// <summary>
    /// Job arguments as JSON.
    /// </summary>
    public string? ArgumentsJson { get; set; }

    /// <summary>
    /// When the schedule should run next.
    /// </summary>
    public DateTimeOffset? NextRunTime { get; set; }

    /// <summary>
    /// When the schedule last ran.
    /// </summary>
    public DateTimeOffset? LastRunTime { get; set; }

    /// <summary>
    /// Whether the schedule is enabled.
    /// </summary>
    public bool IsEnabled { get; set; } = true;

    /// <summary>
    /// When the schedule was created.
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}
