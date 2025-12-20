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
    public static string Job(string prefix, Guid jobId) => $"{prefix}:job:{jobId}";
    public static string JobsByQueue(string prefix, string queue) => $"{prefix}:jobs:queue:{queue}";
    public static string JobsByStatus(string prefix, string status) => $"{prefix}:jobs:status:{status}";
    public static string JobHistory(string prefix, Guid jobId) => $"{prefix}:job:{jobId}:history";
    public static string JobLogs(string prefix, Guid jobId) => $"{prefix}:job:{jobId}:logs";

    // Schedule cache keys
    public static string Schedule(string prefix, Guid scheduleId) => $"{prefix}:schedule:{scheduleId}";
    public static string AllSchedules(string prefix) => $"{prefix}:schedules:all";
    public static string SchedulesByQueue(string prefix, string queue) => $"{prefix}:schedules:queue:{queue}";

    // Worker cache keys
    public static string Worker(string prefix, string workerId) => $"{prefix}:worker:{workerId}";
    public static string AllWorkers(string prefix) => $"{prefix}:workers:all";
    public static string WorkersByQueue(string prefix, string queue) => $"{prefix}:workers:queue:{queue}";

    // Coordinator node cache keys
    public static string CoordinatorNode(string prefix, string nodeId) => $"{prefix}:node:{nodeId}";
    public static string AllCoordinatorNodes(string prefix) => $"{prefix}:nodes:all";
    public static string LeaderNode(string prefix) => $"{prefix}:node:leader";
    public static string FollowerNodes(string prefix) => $"{prefix}:nodes:followers";
}
