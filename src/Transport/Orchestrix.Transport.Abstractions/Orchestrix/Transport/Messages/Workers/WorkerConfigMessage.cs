namespace Orchestrix.Transport.Messages.Workers;

/// <summary>
/// Configuration response from Coordinator to Worker after registration.
/// </summary>
public class WorkerConfigMessage
{
    /// <summary>
    /// The worker ID this configuration is for.
    /// </summary>
    public string WorkerId { get; set; } = string.Empty;

    /// <summary>
    /// Whether the worker registration was accepted.
    /// </summary>
    public bool Accepted { get; set; } = true;

    /// <summary>
    /// Reason for rejection if not accepted.
    /// </summary>
    public string? RejectionReason { get; set; }

    /// <summary>
    /// Configured heartbeat interval from Coordinator.
    /// </summary>
    public TimeSpan HeartbeatInterval { get; set; } = TimeSpan.FromSeconds(10);

    /// <summary>
    /// Default job execution timeout from Coordinator.
    /// </summary>
    public TimeSpan DefaultJobTimeout { get; set; } = TimeSpan.FromMinutes(5);

    /// <summary>
    /// Timeout for worker to be considered dead if no heartbeat.
    /// </summary>
    public TimeSpan WorkerTimeout { get; set; } = TimeSpan.FromSeconds(30);

    /// <summary>
    /// Timestamp when configuration was generated.
    /// </summary>
    public DateTimeOffset Timestamp { get; set; } = DateTimeOffset.UtcNow;
}
