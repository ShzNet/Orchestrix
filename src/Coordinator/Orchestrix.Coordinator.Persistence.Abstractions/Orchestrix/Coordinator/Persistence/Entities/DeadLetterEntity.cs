namespace Orchestrix.Coordinator.Persistence.Entities;

/// <summary>
/// Represents a job that has failed after all retry attempts.
/// </summary>
public class DeadLetterEntity
{
    /// <summary>
    /// Unique dead letter identifier.
    /// </summary>
    public required Guid Id { get; init; }

    /// <summary>
    /// Original job ID that failed.
    /// </summary>
    public required Guid OriginalJobId { get; set; }

    /// <summary>
    /// Job type.
    /// </summary>
    public required string JobType { get; set; }

    /// <summary>
    /// Queue name.
    /// </summary>
    public required string Queue { get; set; }

    /// <summary>
    /// Job arguments as JSON.
    /// </summary>
    public required string ArgumentsJson { get; set; }

    /// <summary>
    /// Last error message before giving up.
    /// </summary>
    public required string LastError { get; set; }

    /// <summary>
    /// Total number of attempts made.
    /// </summary>
    public int TotalAttempts { get; set; }

    /// <summary>
    /// When the job was moved to dead letter.
    /// </summary>
    public DateTimeOffset FailedAt { get; set; } = DateTimeOffset.UtcNow;
}
