using Microsoft.Extensions.DependencyInjection;

namespace Orchestrix.Coordinator;

/// <summary>
/// Runtime configuration options for Coordinator.
/// </summary>
public class CoordinatorOptions
{
    /// <summary>
    /// Unique identifier for this coordinator node.
    /// </summary>
    public string NodeId { get; set; } = Environment.MachineName;

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
}

/// <summary>
/// Builder interface for configuring persistence.
/// Extension methods should be defined in persistence packages (e.g., UseEfCore in Orchestrix.Persistence.EfCore).
/// </summary>
public interface IPersistenceBuilder
{
    /// <summary>
    /// Gets the service collection.
    /// </summary>
    IServiceCollection Services { get; }
}
