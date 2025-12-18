namespace Orchestrix.Jobs;

using Orchestrix.Enums;

/// <summary>
/// Represents information about a job.
/// </summary>
public class JobInfo
{
    public Guid Id { get; set; }
    public string JobType { get; set; } = string.Empty;
    public string Queue { get; set; } = string.Empty;
    public JobStatus Status { get; set; }
    public JobPriority Priority { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? ScheduledAt { get; set; }
    public DateTimeOffset? StartedAt { get; set; }
    public DateTimeOffset? CompletedAt { get; set; }
    public int RetryCount { get; set; }
    public int MaxRetries { get; set; }
    public string? Error { get; set; }
    public string? CorrelationId { get; set; }
}
