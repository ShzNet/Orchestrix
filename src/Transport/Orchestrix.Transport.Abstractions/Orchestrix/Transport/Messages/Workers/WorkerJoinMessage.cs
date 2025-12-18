namespace Orchestrix.Transport.Messages.Workers;

/// <summary>
/// Message for worker join/registration.
/// Broadcast to all coordinators.
/// </summary>
public class WorkerJoinMessage
{
    public string WorkerId { get; set; } = string.Empty;
    public string HostName { get; set; } = string.Empty;
    public string[] Queues { get; set; } = Array.Empty<string>();
    public int MaxConcurrency { get; set; }
    public Dictionary<string, string> Metadata { get; set; } = new();
    public DateTimeOffset Timestamp { get; set; }
}
