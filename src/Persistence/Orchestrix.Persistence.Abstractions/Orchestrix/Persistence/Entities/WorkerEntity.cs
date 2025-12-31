namespace Orchestrix.Persistence.Entities;

/// <summary>
/// Represents a worker node in the cluster.
/// </summary>
public class WorkerEntity
{
    /// <summary>
    /// Unique worker identifier.
    /// </summary>
    public string WorkerId { get; set; } = string.Empty;

    /// <summary>
    /// Queues this worker can process.
    /// </summary>
    public string[] Queues { get; set; } = [];

    /// <summary>
    /// Maximum concurrent jobs this worker can handle.
    /// </summary>
    public int MaxConcurrency { get; set; } = 1;

    /// <summary>
    /// Current number of jobs being processed.
    /// </summary>
    public int CurrentLoad { get; set; } = 0;

    /// <summary>
    /// Last heartbeat timestamp.
    /// </summary>
    public DateTimeOffset LastHeartbeat { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// Worker status.
    /// </summary>
    public WorkerStatus Status { get; set; } = WorkerStatus.Active;

    /// <summary>
    /// Additional metadata as JSON.
    /// </summary>
    public string? Metadata { get; set; }

    /// <summary>
    /// When the worker first registered.
    /// </summary>
    public DateTimeOffset RegisteredAt { get; set; } = DateTimeOffset.UtcNow;
}

/// <summary>
/// Worker status enumeration.
/// </summary>
public enum WorkerStatus
{
    /// <summary>
    /// Worker is active and processing jobs.
    /// </summary>
    Active,

    /// <summary>
    /// Worker is draining (finishing current jobs, not accepting new ones).
    /// </summary>
    Draining,

    /// <summary>
    /// Worker is offline/dead.
    /// </summary>
    Offline
}
