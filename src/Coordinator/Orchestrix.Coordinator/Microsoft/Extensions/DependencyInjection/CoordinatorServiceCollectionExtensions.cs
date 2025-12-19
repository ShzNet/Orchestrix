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
        var transportBuilder = new TransportConfigurationBuilder(services);
        var lockingBuilder = new LockingConfigurationBuilder(services);
        var persistenceBuilder = new PersistenceConfigurationBuilder(services);

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

        // Register leader election
        services.AddSingleton<Orchestrix.Coordinator.LeaderElection.ILeaderElection, Orchestrix.Coordinator.LeaderElection.LeaderElection>();
        services.AddHostedService<Orchestrix.Coordinator.LeaderElection.LeaderElectionHostedService>();

        // Register scheduling services
        services.AddSingleton<Orchestrix.Coordinator.Scheduling.IScheduler, Orchestrix.Coordinator.Scheduling.Scheduler>();
        services.AddSingleton<Orchestrix.Coordinator.Scheduling.JobPlanner>();
        services.AddHostedService<Orchestrix.Coordinator.Scheduling.ScheduleScanner>();

        // Register dispatching services
        services.AddSingleton<Orchestrix.Coordinator.Dispatching.IJobDispatcher, Orchestrix.Coordinator.Dispatching.JobDispatcher>();

        // TODO: Register core services (will be added in later stages)
        // - CoordinatorService (IHostedService)
        // - Background services
        // - Handlers

        return new CoordinatorBuilder(services);
    }
}
