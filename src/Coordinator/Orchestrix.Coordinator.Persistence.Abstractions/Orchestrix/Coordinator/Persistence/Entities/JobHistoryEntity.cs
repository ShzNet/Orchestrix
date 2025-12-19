namespace Orchestrix.Coordinator.Persistence.Entities;

/// <summary>
/// Represents a job execution history record.
/// One record is created for each retry attempt.
/// </summary>
public class JobHistoryEntity
{
    /// <summary>
    /// Unique history record identifier.
    /// </summary>
    public required Guid Id { get; init; }

    /// <summary>
    /// Reference to the parent job.
    /// </summary>
    public required Guid JobId { get; set; }

    /// <summary>
    /// Attempt number (1-based).
    /// </summary>
    public required int AttemptNumber { get; set; }

    /// <summary>
    /// ID of the worker that executed this attempt.
    /// </summary>
    public string? WorkerId { get; set; }

    /// <summary>
    /// When this attempt started.
    /// </summary>
    public DateTimeOffset? StartedAt { get; set; }

    /// <summary>
    /// When this attempt completed.
    /// </summary>
    public DateTimeOffset? CompletedAt { get; set; }

    /// <summary>
    /// Final status of this attempt.
    /// </summary>
    public JobStatus Status { get; set; }

    /// <summary>
    /// Error message if this attempt failed.
    /// </summary>
    public string? Error { get; set; }

    /// <summary>
    /// Execution duration.
    /// </summary>
    public TimeSpan? Duration { get; set; }
}
