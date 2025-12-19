namespace Orchestrix.Coordinator.Persistence.Entities;

/// <summary>
/// Represents a job in the coordinator's persistence layer.
/// </summary>
public class JobEntity
{
    /// <summary>
    /// Unique job identifier.
    /// </summary>
    public required Guid Id { get; init; }

    /// <summary>
    /// Fully qualified job type name (e.g., "MyApp.Jobs.SendEmailJob").
    /// </summary>
    public required string JobType { get; set; }

    /// <summary>
    /// Queue name for job routing.
    /// </summary>
    public required string Queue { get; set; }

    /// <summary>
    /// Job arguments as JSON.
    /// </summary>
    public required string ArgumentsJson { get; set; }

    /// <summary>
    /// Current job status.
    /// </summary>
    public required JobStatus Status { get; set; }

    /// <summary>
    /// Job priority (higher = more important).
    /// </summary>
    public int Priority { get; set; } = 0;

    /// <summary>
    /// Correlation ID for tracking related jobs.
    /// </summary>
    public string? CorrelationId { get; set; }

    /// <summary>
    /// When the job was created.
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// When the job should be executed (for delayed jobs).
    /// </summary>
    public DateTimeOffset? ScheduledAt { get; set; }

    /// <summary>
    /// When the job was dispatched to a worker.
    /// </summary>
    public DateTimeOffset? DispatchedAt { get; set; }

    /// <summary>
    /// When the job started executing.
    /// </summary>
    public DateTimeOffset? StartedAt { get; set; }

    /// <summary>
    /// When the job completed (success or failure).
    /// </summary>
    public DateTimeOffset? CompletedAt { get; set; }

    /// <summary>
    /// Maximum execution timeout.
    /// </summary>
    public TimeSpan? Timeout { get; set; }

    /// <summary>
    /// Current retry attempt number.
    /// </summary>
    public int RetryCount { get; set; } = 0;

    /// <summary>
    /// Maximum number of retry attempts.
    /// </summary>
    public int MaxRetries { get; set; } = 0;

    /// <summary>
    /// Retry policy configuration as JSON.
    /// </summary>
    public string? RetryPolicyJson { get; set; }

    /// <summary>
    /// Last error message (if failed).
    /// </summary>
    public string? Error { get; set; }

    /// <summary>
    /// ID of the worker currently executing this job.
    /// </summary>
    public string? WorkerId { get; set; }

    /// <summary>
    /// ID of the Coordinator node (follower) that owns this job for event processing.
    /// Used for follower coordination pattern.
    /// </summary>
    public string? FollowerNodeId { get; set; }

    /// <summary>
    /// Whether job-specific channels have been cleaned up.
    /// Used to track cleanup of job.{id}.status and job.{id}.logs channels.
    /// </summary>
    public bool ChannelsCleaned { get; set; } = false;
}
