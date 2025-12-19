using Microsoft.Extensions.DependencyInjection;
using Orchestrix.Locking;
using Orchestrix.Transport;

namespace Orchestrix.Coordinator;

/// <summary>
/// Configuration options for Coordinator.
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
    /// Gets the service collection for nested configuration.
    /// </summary>
    public IServiceCollection Services { get; set; } = null!;

    /// <summary>
    /// Transport configuration builder.
    /// </summary>
    public ITransportBuilder Transport { get; set; } = null!;

    /// <summary>
    /// Locking configuration builder.
    /// </summary>
    public ILockingBuilder Locking { get; set; } = null!;

    /// <summary>
    /// Persistence configuration builder.
    /// </summary>
    public IPersistenceBuilder Persistence { get; set; } = null!;
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
