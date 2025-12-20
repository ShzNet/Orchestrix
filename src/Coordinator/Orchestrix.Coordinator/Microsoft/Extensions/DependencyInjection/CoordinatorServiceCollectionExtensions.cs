using Orchestrix.Coordinator;
using Orchestrix.Coordinator.Dispatching;
using Orchestrix.Coordinator.LeaderElection;
using Orchestrix.Coordinator.QueueScanning;
using Orchestrix.Coordinator.Scheduling;

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
        services.AddSingleton<ILeaderElection, LeaderElection>();
        services.AddHostedService<LeaderElectionHostedService>();

        // Register scheduling services
        services.AddSingleton<IScheduler, Scheduler>();
        services.AddSingleton<JobPlanner>();
        services.AddHostedService<ScheduleScanner>();

        // Register dispatching services
        services.AddSingleton<IJobDispatcher, JobDispatcher>();

        // Register queue scanning services
        services.AddHostedService<JobQueueScanner>();

        // Register cache invalidation service
        services.AddSingleton<Orchestrix.Coordinator.Caching.ICacheInvalidator, Orchestrix.Coordinator.Services.CacheInvalidator>();

        // TODO: Register core services (will be added in later stages)
        // - CoordinatorService (IHostedService)
        // - Background services
        // - Handlers

        return new CoordinatorBuilder(services);
    }
}
