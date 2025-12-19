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
    public ITransportConfigurationBuilder Transport { get; }

    /// <summary>
    /// Locking configuration builder.
    /// </summary>
    public ILockingConfigurationBuilder Locking { get; }

    /// <summary>
    /// Persistence configuration builder.
    /// </summary>
    public IPersistenceConfigurationBuilder Persistence { get; }

    /// <summary>
    /// Runtime options for Coordinator.
    /// </summary>
    public CoordinatorOptions Options { get; }

    /// <summary>
    /// Initializes a new instance of CoordinatorConfiguration.
    /// </summary>
    public CoordinatorConfiguration(
        IServiceCollection services,
        ITransportConfigurationBuilder transport,
        ILockingConfigurationBuilder locking,
        IPersistenceConfigurationBuilder persistence,
        CoordinatorOptions options)
    {
        Services = services;
        Transport = transport;
        Locking = locking;
        Persistence = persistence;
        Options = options;
    }
}
