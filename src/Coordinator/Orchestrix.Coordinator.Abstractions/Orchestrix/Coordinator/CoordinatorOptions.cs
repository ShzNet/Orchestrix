using Microsoft.Extensions.DependencyInjection;

namespace Orchestrix.Coordinator;

/// <summary>
/// Runtime configuration options for Coordinator.
/// </summary>
public class CoordinatorOptions
{
    /// <summary>
    /// Unique identifier for this coordinator node.
    /// Defaults to a random GUID if not specified.
    /// </summary>
    public string NodeId { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// Interval between heartbeat/metrics broadcasts.
    /// </summary>
    public TimeSpan HeartbeatInterval { get; set; } = TimeSpan.FromSeconds(10);

    /// <summary>
    /// Duration of leader lease (how long leader holds the lock).
    /// </summary>
    public TimeSpan LeaderLeaseDuration { get; set; } = TimeSpan.FromSeconds(30);

    /// <summary>
    /// Interval for leader to renew its lease.
    /// Should be less than LeaderLeaseDuration.
    /// </summary>
    public TimeSpan LeaderRenewInterval { get; set; } = TimeSpan.FromSeconds(10);

    /// <summary>
    /// Timeout to consider a node as dead (based on last heartbeat).
    /// </summary>
    public TimeSpan NodeTimeout { get; set; } = TimeSpan.FromSeconds(30);

    /// <summary>
    /// Interval for leader to check for dead nodes.
    /// </summary>
    public TimeSpan DeadNodeCheckInterval { get; set; } = TimeSpan.FromSeconds(15);

    /// <summary>
    /// Prefix for cache keys (allows multiple Orchestrix instances on same cache).
    /// </summary>
    public string CachePrefix { get; set; } = "orchestrix";

    /// <summary>
    /// Interval for scanning cron schedules.
    /// </summary>
    public TimeSpan ScheduleScanInterval { get; set; } = TimeSpan.FromSeconds(10);

    /// <summary>
    /// Interval for scanning pending job queue.
    /// </summary>
    public TimeSpan JobQueueScanInterval { get; set; } = TimeSpan.FromSeconds(5);

    /// <summary>
    /// Maximum number of jobs to dispatch per scan iteration.
    /// </summary>
    public int JobQueueBatchSize { get; set; } = 100;

    /// <summary>
    /// Default heartbeat interval to configure for workers.
    /// </summary>
    public TimeSpan DefaultWorkerHeartbeatInterval { get; set; } = TimeSpan.FromSeconds(10);

    /// <summary>
    /// Default job timeout to configure for workers.
    /// </summary>
    public TimeSpan DefaultWorkerJobTimeout { get; set; } = TimeSpan.FromMinutes(5);

    /// <summary>
    /// Timeout to consider a worker as dead (no heartbeat).
    /// </summary>
    public TimeSpan DefaultWorkerTimeout { get; set; } = TimeSpan.FromSeconds(30);
}
