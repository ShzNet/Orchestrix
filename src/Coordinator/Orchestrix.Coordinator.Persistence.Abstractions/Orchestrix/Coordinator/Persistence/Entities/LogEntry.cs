namespace Orchestrix.Coordinator.Persistence.Entities;

/// <summary>
/// Represents a job execution log entry.
/// </summary>
public class LogEntry
{
    /// <summary>
    /// Unique log entry identifier.
    /// </summary>
    public required Guid Id { get; init; }

    /// <summary>
    /// Reference to the job.
    /// </summary>
    public required Guid JobId { get; set; }

    /// <summary>
    /// When the log entry was created.
    /// </summary>
    public DateTimeOffset Timestamp { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// Log level (e.g., "Info", "Warning", "Error").
    /// </summary>
    public required string Level { get; set; }

    /// <summary>
    /// Log message.
    /// </summary>
    public required string Message { get; set; }

    /// <summary>
    /// Exception details if applicable.
    /// </summary>
    public string? Exception { get; set; }
}
