using Orchestrix.Coordinator;
using Orchestrix.Coordinator.Core.Builders;
using Orchestrix.Coordinator.HostedServices.Clustering;
using Orchestrix.Coordinator.Services.Clustering;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Orchestrix.Logging.Persistence;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods for setting up Orchestrix Coordinator services.
/// </summary>
public static class CoordinatorServiceCollectionExtensions
{
    /// <summary>
    /// Adds Orchestrix Coordinator services to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <param name="configure">Delegate to configure the coordinator.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddOrchestrixCoordinator(this IServiceCollection services, Action<CoordinatorConfiguration> configure)
    {
        services.AddOptions();
        services.AddSingleton(new CoordinatorOptions());

        var options = new CoordinatorOptions();
        
        var transport = new TransportConfigurationBuilder(services);
        var locking = new LockingConfigurationBuilder(services);
        var persistence = new PersistenceConfigurationBuilder(services);
        var logging = new LoggingConfigurationBuilder(services);

        var config = new CoordinatorConfiguration(services, transport, locking, persistence, logging, options);

        configure(config);

        // Register Options (singleton for now, or use IOptions pattern properly later)
        services.AddSingleton(options);

        // Register Core Services
        services.AddSingleton<LeaderElection>();
        // services.AddSingleton<ILeaderElection>(sp => sp.GetRequiredService<LeaderElection>()); // If needed
        services.AddSingleton<ILeaderElection>(sp => sp.GetRequiredService<LeaderElection>());
        services.AddHostedService<LeaderElectionHostedService>();
        services.AddHostedService<NodeHeartbeatHostedService>();
        
        // Default Logging
        services.TryAddScoped<ILogStore, Orchestrix.Coordinator.Services.LoggerLogStore>();

        return services;
    }
}
