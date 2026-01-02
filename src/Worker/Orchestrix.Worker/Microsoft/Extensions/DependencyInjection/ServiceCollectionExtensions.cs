using Microsoft.Extensions.DependencyInjection.Extensions;
using Orchestrix.Jobs;
using Orchestrix.Worker;
using Orchestrix.Worker.Consumer;
using Orchestrix.Worker.Execution;
using Orchestrix.Worker.Heartbeat;
using Orchestrix.Worker.Registration;
using Orchestrix.Worker.Registry;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods for configuring Worker services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds Orchestrix Worker services to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configureOptions">Action to configure worker options.</param>
    /// <param name="configureBuilder">Action to register job handlers and middlewares.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddOrchestrixWorker(
        this IServiceCollection services,
        Action<WorkerOptions>? configureOptions = null,
        Action<IWorkerBuilder>? configureBuilder = null)
    {
        // Create and configure options
        var options = new WorkerOptions();
        configureOptions?.Invoke(options);
        services.TryAddSingleton(options);

        // Register runtime config (updated after registration)
        services.TryAddSingleton<WorkerRuntimeConfig>();

        // Create and configure builder
        var builder = new WorkerBuilder(services);
        configureBuilder?.Invoke(builder);

        // Store middleware configuration for JobExecutor
        var middlewareTypes = builder.MiddlewareTypes.ToList();
        var inlineMiddlewares = builder.InlineMiddlewares.ToList();

        // Register handlers from builder into registry
        services.AddSingleton<IJobHandlerRegistry>(sp =>
        {
            var registry = new JobHandlerRegistry(sp.GetRequiredService<Microsoft.Extensions.Logging.ILogger<JobHandlerRegistry>>());
            foreach (var (jobType, handlerType, argsType) in builder.Handlers)
            {
                registry.RegisterHandler(jobType, handlerType, argsType);
            }
            return registry;
        });

        // Register JobExecutor with middleware configuration
        services.AddSingleton<IJobExecutor>(sp => new JobExecutor(
            sp,
            sp.GetRequiredService<IJobHandlerRegistry>(),
            sp.GetRequiredService<Orchestrix.Transport.IPublisher>(),
            sp.GetRequiredService<Orchestrix.Transport.TransportChannels>(),
            sp.GetRequiredService<WorkerOptions>(),
            middlewareTypes,
            inlineMiddlewares,
            sp.GetRequiredService<Microsoft.Extensions.Logging.ILogger<JobExecutor>>()
        ));

        // Register registration service (must start first)
        services.AddSingleton<WorkerRegistrationService>();
        services.AddHostedService(sp => sp.GetRequiredService<WorkerRegistrationService>());

        // Register other hosted services (wait for registration)
        services.AddSingleton<JobConsumerService>();
        services.AddHostedService(sp => sp.GetRequiredService<JobConsumerService>());
        services.AddHostedService<HeartbeatService>();

        return services;
    }
}
