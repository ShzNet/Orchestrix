namespace Orchestrix.Transport.Messages.Workers;

/// <summary>
/// Message for worker shutdown notification.
/// Sent to assigned coordinator.
/// </summary>
public class WorkerShutdownMessage
{
    public string WorkerId { get; set; } = string.Empty;
    public string? Reason { get; set; }
    public bool Graceful { get; set; }
    public DateTimeOffset Timestamp { get; set; }
}
