namespace Orchestrix.Coordinator.Caching;

/// <summary>
/// Shared cache key constants for Coordinator and Control Panel.
/// Ensures consistent cache key generation across all components.
/// </summary>
public static class CacheKeys
{
    /// <summary>
    /// Cache key prefix for all Orchestrix coordinator cache entries.
    /// Can be configured via CoordinatorOptions.CachePrefix.
    /// </summary>
    public const string DefaultPrefix = "orchestrix:coordinator";

    // Job cache keys

    /// <summary>
    /// Generates key for a specific job.
    /// </summary>
    public static string Job(string prefix, Guid jobId) => $"{prefix}:job:{jobId}";

    /// <summary>
    /// Generates key for jobs list by queue.
    /// </summary>
    public static string JobsByQueue(string prefix, string queue) => $"{prefix}:jobs:queue:{queue}";

    /// <summary>
    /// Generates key for jobs list by status.
    /// </summary>
    public static string JobsByStatus(string prefix, string status) => $"{prefix}:jobs:status:{status}";

    /// <summary>
    /// Generates key for job history.
    /// </summary>
    public static string JobHistory(string prefix, Guid jobId) => $"{prefix}:job:{jobId}:history";

    /// <summary>
    /// Generates key for job logs.
    /// </summary>
    public static string JobLogs(string prefix, Guid jobId) => $"{prefix}:job:{jobId}:logs";

    // Schedule cache keys

    /// <summary>
    /// Generates key for a specific schedule.
    /// </summary>
    public static string Schedule(string prefix, Guid scheduleId) => $"{prefix}:schedule:{scheduleId}";

    /// <summary>
    /// Generates key for all schedules list.
    /// </summary>
    public static string AllSchedules(string prefix) => $"{prefix}:schedules:all";

    /// <summary>
    /// Generates key for schedules list by queue.
    /// </summary>
    public static string SchedulesByQueue(string prefix, string queue) => $"{prefix}:schedules:queue:{queue}";

    // Worker cache keys

    /// <summary>
    /// Generates key for a specific worker.
    /// </summary>
    public static string Worker(string prefix, string workerId) => $"{prefix}:worker:{workerId}";

    /// <summary>
    /// Generates key for all workers list.
    /// </summary>
    public static string AllWorkers(string prefix) => $"{prefix}:workers:all";

    /// <summary>
    /// Generates key for workers list by queue.
    /// </summary>
    public static string WorkersByQueue(string prefix, string queue) => $"{prefix}:workers:queue:{queue}";

    // Coordinator node cache keys

    /// <summary>
    /// Generates key for a specific coordinator node.
    /// </summary>
    public static string CoordinatorNode(string prefix, string nodeId) => $"{prefix}:node:{nodeId}";

    /// <summary>
    /// Generates key for all coordinator nodes list.
    /// </summary>
    public static string AllCoordinatorNodes(string prefix) => $"{prefix}:nodes:all";

    /// <summary>
    /// Generates key for the leader node.
    /// </summary>
    public static string LeaderNode(string prefix) => $"{prefix}:node:leader";

    /// <summary>
    /// Generates key for follower nodes list.
    /// </summary>
    public static string FollowerNodes(string prefix) => $"{prefix}:nodes:followers";
}
