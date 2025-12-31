namespace Orchestrix.Jobs;

using Orchestrix.Enums;

/// <summary>
/// Represents information about a job.
/// </summary>
public class JobInfo
{
    /// <summary>
    /// Gets or sets the unique identifier of the job.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the type of the job.
    /// </summary>
    public string JobType { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the name of the queue the job belongs to.
    /// </summary>
    public string Queue { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the current status of the job.
    /// </summary>
    public JobStatus Status { get; set; }

    /// <summary>
    /// Gets or sets the priority of the job.
    /// </summary>
    public JobPriority Priority { get; set; }

    /// <summary>
    /// Gets or sets the timestamp when the job was created.
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the scheduled execution time for the job, if applicable.
    /// </summary>
    public DateTimeOffset? ScheduledAt { get; set; }

    /// <summary>
    /// Gets or sets the timestamp when the job started execution.
    /// </summary>
    public DateTimeOffset? StartedAt { get; set; }

    /// <summary>
    /// Gets or sets the timestamp when the job was completed (successfully or failed).
    /// </summary>
    public DateTimeOffset? CompletedAt { get; set; }

    /// <summary>
    /// Gets or sets the number of times the job has been retried.
    /// </summary>
    public int RetryCount { get; set; }

    /// <summary>
    /// Gets or sets the maximum number of retries allowed for the job.
    /// </summary>
    public int MaxRetries { get; set; }

    /// <summary>
    /// Gets or sets the error message if the job failed.
    /// </summary>
    public string? Error { get; set; }

    /// <summary>
    /// Gets or sets the correlation identifier for tracking the job across systems.
    /// </summary>
    public string? CorrelationId { get; set; }
}
