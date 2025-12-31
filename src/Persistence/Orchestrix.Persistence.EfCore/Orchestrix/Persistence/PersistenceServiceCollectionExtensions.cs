using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Orchestrix.Persistence.EfCore;
using Orchestrix.Persistence.EfCore.Stores;

namespace Orchestrix.Persistence;

/// <summary>
/// Extension methods for configuring persistence services.
/// </summary>
public static class PersistenceServiceCollectionExtensions
{
    /// <summary>
    /// Configures the coordinator to use Entity Framework Core for persistence.
    /// </summary>
    /// <typeparam name="TContext">The concrete DbContext type.</typeparam>
    /// <param name="builder">The persistence configuration builder.</param>
    /// <param name="configure">Delegate to configure the DbContext options.</param>
    /// <returns>The persistence configuration builder.</returns>
    public static IPersistenceConfigurationBuilder UseEfCore<TContext>(this IPersistenceConfigurationBuilder builder, Action<DbContextOptionsBuilder>? configure = null)
        where TContext : CoordinatorDbContext
    {
        builder.Services.AddDbContext<TContext>(options =>
        {
            if (configure != null)
            {
                configure(options);
            }
        });

        // Register the abstract CoordinatorDbContext to resolve to TContext
        builder.Services.TryAddScoped<CoordinatorDbContext>(sp => sp.GetRequiredService<TContext>());

        builder.Services.TryAddScoped<IJobStore, JobStore>();
        builder.Services.TryAddScoped<IWorkerStore, WorkerStore>();
        builder.Services.TryAddScoped<ICoordinatorNodeStore, CoordinatorNodeStore>();
        builder.Services.TryAddScoped<ICronScheduleStore, CronScheduleStore>();
        builder.Services.TryAddScoped<IDeadLetterStore, DeadLetterStore>();
        builder.Services.TryAddScoped<IJobHistoryStore, JobHistoryStore>();

        return builder;
    }
}
