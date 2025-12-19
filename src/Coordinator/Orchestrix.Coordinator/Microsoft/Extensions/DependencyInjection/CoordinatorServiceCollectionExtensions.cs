using Microsoft.Extensions.DependencyInjection;
using Orchestrix.Coordinator;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods for registering Orchestrix Coordinator services.
/// </summary>
public static class CoordinatorServiceCollectionExtensions
{
    /// <summary>
    /// Adds Orchestrix Coordinator services to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configure">Configuration action for Coordinator.</param>
    /// <returns>A builder for further configuration.</returns>
    public static ICoordinatorConfigurationBuilder AddOrchestrixCoordinator(
        this IServiceCollection services,
        Action<CoordinatorConfiguration>? configure = null)
    {
        // Create options
        var options = new CoordinatorOptions();

        // Create builders
        var transportBuilder = new TransportBuilder(services);
        var lockingBuilder = new LockingBuilder(services);
        var persistenceBuilder = new PersistenceBuilder(services);

        // Create configuration
        var configuration = new CoordinatorConfiguration(
            services,
            transportBuilder,
            lockingBuilder,
            persistenceBuilder,
            options);

        // Apply user configuration
        configure?.Invoke(configuration);

        // Register options
        services.Configure<CoordinatorOptions>(opt =>
        {
            opt.NodeId = options.NodeId;
            opt.HeartbeatInterval = options.HeartbeatInterval;
            opt.LeaderLeaseDuration = options.LeaderLeaseDuration;
            opt.LeaderRenewInterval = options.LeaderRenewInterval;
            opt.NodeTimeout = options.NodeTimeout;
            opt.DeadNodeCheckInterval = options.DeadNodeCheckInterval;
        });

        // TODO: Register core services (will be added in later stages)
        // - CoordinatorService (IHostedService)
        // - Leader election
        // - Background services
        // - Handlers

        return new CoordinatorBuilder(services);
    }
}
