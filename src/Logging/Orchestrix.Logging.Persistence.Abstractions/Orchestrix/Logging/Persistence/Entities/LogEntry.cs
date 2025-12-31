namespace Orchestrix.Logging.Persistence.Entities;

/// <summary>
/// Represents a job execution log entry.
/// </summary>
public class LogEntry
{
    /// <summary>
    /// Unique log entry identifier.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Reference to the job.
    /// </summary>
    public Guid JobId { get; set; }

    /// <summary>
    /// When the log entry was created.
    /// </summary>
    public DateTimeOffset Timestamp { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// Log level (e.g., "Info", "Warning", "Error").
    /// </summary>
    public string Level { get; set; } = string.Empty;

    /// <summary>
    /// Log message.
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Exception details if applicable.
    /// </summary>
    public string? Exception { get; set; }
}
