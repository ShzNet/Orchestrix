namespace Orchestrix.Coordinator.Persistence.Entities;

/// <summary>
/// Represents an interval-based recurring schedule.
/// </summary>
public class IntervalScheduleEntity
{
    /// <summary>
    /// Unique schedule identifier.
    /// </summary>
    public required Guid Id { get; init; }

    /// <summary>
    /// Human-readable schedule name.
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Interval between executions.
    /// </summary>
    public required TimeSpan Interval { get; set; }

    /// <summary>
    /// Job type to execute.
    /// </summary>
    public required string JobType { get; set; }

    /// <summary>
    /// Queue for job execution.
    /// </summary>
    public required string Queue { get; set; }

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
