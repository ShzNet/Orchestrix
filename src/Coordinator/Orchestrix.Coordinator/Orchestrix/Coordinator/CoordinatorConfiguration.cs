using Microsoft.Extensions.DependencyInjection;
using Orchestrix.Coordinator.Persistence;
using Orchestrix.Locking;
using Orchestrix.Transport;

namespace Orchestrix.Coordinator;

/// <summary>
/// Configuration object for defining Coordinator behavior and dependencies.
/// </summary>
public class CoordinatorConfiguration(
    IServiceCollection services,
    ITransportConfigurationBuilder transport,
    ILockingConfigurationBuilder locking,
    IPersistenceConfigurationBuilder persistence,
    CoordinatorOptions options)
{
    /// <summary>
    /// Gets the service collection.
    /// </summary>
    public IServiceCollection Services { get; } = services;

    /// <summary>
    /// Gets the transport configuration builder.
    /// </summary>
    public ITransportConfigurationBuilder Transport { get; } = transport;

    /// <summary>
    /// Gets the locking configuration builder.
    /// </summary>
    public ILockingConfigurationBuilder Locking { get; } = locking;

    /// <summary>
    /// Gets the persistence configuration builder.
    /// </summary>
    public IPersistenceConfigurationBuilder Persistence { get; } = persistence;

    /// <summary>
    /// Gets the coordinator options.
    /// </summary>
    public CoordinatorOptions Options { get; } = options;
}