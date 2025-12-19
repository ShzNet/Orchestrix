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
    /// <param name="configureOptions">Optional configuration action for CoordinatorOptions.</param>
    /// <returns>A builder for further configuration.</returns>
    public static ICoordinatorBuilder AddOrchestrixCoordinator(
        this IServiceCollection services,
        Action<CoordinatorOptions>? configureOptions = null)
    {
        // Register options with nested builders
        services.Configure<CoordinatorOptions>(opt =>
        {
            // Initialize nested builders
            opt.Services = services;
            opt.Transport = new TransportBuilder(services);
            opt.Locking = new LockingBuilder(services);
            opt.Persistence = new PersistenceBuilder(services);

            // Apply user configuration
            configureOptions?.Invoke(opt);
        });

        // TODO: Register core services (will be added in later stages)
        // - CoordinatorService (IHostedService)
        // - Leader election
        // - Background services
        // - Handlers

        return new CoordinatorBuilder(services);
    }
}
