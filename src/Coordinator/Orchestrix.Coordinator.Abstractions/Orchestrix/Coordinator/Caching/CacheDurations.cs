namespace Orchestrix.Coordinator.Caching;

/// <summary>
/// Shared cache duration constants for Coordinator and Control Panel.
/// Defines TTL (Time-To-Live) for different types of cached data.
/// </summary>
public static class CacheDurations
{
    /// <summary>
    /// Job data cache duration (30 seconds).
    /// Jobs change frequently, so short TTL.
    /// </summary>
    public static readonly TimeSpan Job = TimeSpan.FromSeconds(30);

    /// <summary>
    /// Job history cache duration (5 minutes).
    /// History is immutable once written, can cache longer.
    /// </summary>
    public static readonly TimeSpan JobHistory = TimeSpan.FromMinutes(5);

    /// <summary>
    /// Job logs cache duration (1 minute).
    /// Logs are append-only but updated frequently during execution.
    /// </summary>
    public static readonly TimeSpan JobLogs = TimeSpan.FromMinutes(1);

    /// <summary>
    /// Schedule data cache duration (5 minutes).
    /// Schedules change infrequently.
    /// </summary>
    public static readonly TimeSpan Schedule = TimeSpan.FromMinutes(5);

    /// <summary>
    /// Worker data cache duration (10 seconds).
    /// Worker status changes frequently (heartbeats, load).
    /// </summary>
    public static readonly TimeSpan Worker = TimeSpan.FromSeconds(10);

    /// <summary>
    /// Coordinator node data cache duration (10 seconds).
    /// Node status changes frequently (heartbeats, role changes).
    /// </summary>
    public static readonly TimeSpan CoordinatorNode = TimeSpan.FromSeconds(10);

    /// <summary>
    /// Leader node cache duration (5 seconds).
    /// Leader election can change, need fresh data.
    /// </summary>
    public static readonly TimeSpan LeaderNode = TimeSpan.FromSeconds(5);
}
