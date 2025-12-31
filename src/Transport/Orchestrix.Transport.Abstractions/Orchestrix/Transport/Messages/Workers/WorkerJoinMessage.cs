namespace Orchestrix.Transport.Messages.Workers;

/// <summary>
/// Message for worker join/registration.
/// Broadcast to all coordinators.
/// </summary>
public class WorkerJoinMessage
{
    /// <summary>
    /// The unique identifier of the worker.
    /// </summary>
    public string WorkerId { get; set; } = string.Empty;

    /// <summary>
    /// The hostname where the worker is running.
    /// </summary>
    public string HostName { get; set; } = string.Empty;

    /// <summary>
    /// The list of queues this worker is listening to.
    /// </summary>
    public string[] Queues { get; set; } = Array.Empty<string>();

    /// <summary>
    /// The maximum number of concurrent jobs this worker can handle.
    /// </summary>
    public int MaxConcurrency { get; set; }

    /// <summary>
    /// Additional metadata associated with the worker.
    /// </summary>
    public Dictionary<string, string> Metadata { get; set; } = new();

    /// <summary>
    /// The timestamp when the worker joined.
    /// </summary>
    public DateTimeOffset Timestamp { get; set; }
}
