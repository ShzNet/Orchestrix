namespace Orchestrix.Jobs;

using Orchestrix.Enums;

/// <summary>
/// Represents information about a job.
/// </summary>
public record JobInfo
{
    /// <summary>
    /// Gets the unique identifier of the job.
    /// </summary>
    public required Guid Id { get; init; }

    /// <summary>
    /// Gets the type/name of the job.
    /// </summary>
    public required string JobType { get; init; }

    /// <summary>
    /// Gets the queue name where the job is dispatched.
    /// </summary>
    public required string Queue { get; init; }

    /// <summary>
    /// Gets the current status of the job.
    /// </summary>
    public required JobStatus Status { get; init; }

    /// <summary>
    /// Gets the priority level of the job.
    /// </summary>
    public required JobPriority Priority { get; init; }

    /// <summary>
    /// Gets the timestamp when the job was created.
    /// </summary>
    public DateTimeOffset CreatedAt { get; init; }

    /// <summary>
    /// Gets the timestamp when the job is scheduled to run.
    /// </summary>
    public DateTimeOffset? ScheduledAt { get; init; }

    /// <summary>
    /// Gets the timestamp when the job started execution.
    /// </summary>
    public DateTimeOffset? StartedAt { get; init; }

    /// <summary>
    /// Gets the timestamp when the job completed execution.
    /// </summary>
    public DateTimeOffset? CompletedAt { get; init; }

    /// <summary>
    /// Gets the current retry attempt count.
    /// </summary>
    public int RetryCount { get; init; }

    /// <summary>
    /// Gets the maximum number of retry attempts allowed.
    /// </summary>
    public int MaxRetries { get; init; }

    /// <summary>
    /// Gets the error message if the job failed.
    /// </summary>
    public string? Error { get; init; }

    /// <summary>
    /// Gets the correlation ID for tracking related operations.
    /// </summary>
    public string? CorrelationId { get; init; }
}
