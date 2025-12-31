namespace Orchestrix.Transport.Messages.Workers;

/// <summary>
/// Message for worker shutdown notification.
/// Sent to assigned coordinator.
/// </summary>
public class WorkerShutdownMessage
{
    /// <summary>
    /// The unique identifier of the worker.
    /// </summary>
    public string WorkerId { get; set; } = string.Empty;

    /// <summary>
    /// The reason for the shutdown.
    /// </summary>
    public string? Reason { get; set; }

    /// <summary>
    /// Indicates whether the shutdown was graceful.
    /// </summary>
    public bool Graceful { get; set; }

    /// <summary>
    /// The timestamp when the shutdown occurred.
    /// </summary>
    public DateTimeOffset Timestamp { get; set; }
}
