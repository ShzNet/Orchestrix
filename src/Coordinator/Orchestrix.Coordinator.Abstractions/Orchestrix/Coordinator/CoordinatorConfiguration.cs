using Microsoft.Extensions.DependencyInjection;
using Orchestrix.Locking;
using Orchestrix.Transport;

namespace Orchestrix.Coordinator;

/// <summary>
/// Configuration class for Coordinator with nested builders.
/// Used during service registration, not runtime.
/// </summary>
public class CoordinatorConfiguration
{
    /// <summary>
    /// Gets the service collection for nested configuration.
    /// </summary>
    public IServiceCollection Services { get; }

    /// <summary>
    /// Transport configuration builder.
    /// </summary>
    public ITransportBuilder Transport { get; }

    /// <summary>
    /// Locking configuration builder.
    /// </summary>
    public ILockingBuilder Locking { get; }

    /// <summary>
    /// Persistence configuration builder.
    /// </summary>
    public IPersistenceBuilder Persistence { get; }

    /// <summary>
    /// Runtime options for Coordinator.
    /// </summary>
    public CoordinatorOptions Options { get; }

    /// <summary>
    /// Initializes a new instance of CoordinatorConfiguration.
    /// </summary>
    public CoordinatorConfiguration(
        IServiceCollection services,
        ITransportBuilder transport,
        ILockingBuilder locking,
        IPersistenceBuilder persistence,
        CoordinatorOptions options)
    {
        Services = services;
        Transport = transport;
        Locking = locking;
        Persistence = persistence;
        Options = options;
    }
}
