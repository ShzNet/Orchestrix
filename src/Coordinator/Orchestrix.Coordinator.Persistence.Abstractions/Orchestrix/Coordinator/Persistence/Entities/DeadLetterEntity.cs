namespace Orchestrix.Coordinator.Persistence.Entities;

/// <summary>
/// Represents a job that has failed after all retry attempts.
/// </summary>
public class DeadLetterEntity
{
    /// <summary>
    /// Unique dead letter identifier.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Original job ID that failed.
    /// </summary>
    public Guid OriginalJobId { get; set; }

    /// <summary>
    /// Job type.
    /// </summary>
    public string JobType { get; set; } = string.Empty;

    /// <summary>
    /// Queue name.
    /// </summary>
    public string Queue { get; set; } = string.Empty;

    /// <summary>
    /// Job arguments as JSON.
    /// </summary>
    public string ArgumentsJson { get; set; } = string.Empty;

    /// <summary>
    /// Last error message before giving up.
    /// </summary>
    public string LastError { get; set; } = string.Empty;

    /// <summary>
    /// Total number of attempts made.
    /// </summary>
    public int TotalAttempts { get; set; }

    /// <summary>
    /// When the job was moved to dead letter.
    /// </summary>
    public DateTimeOffset FailedAt { get; set; } = DateTimeOffset.UtcNow;
}
