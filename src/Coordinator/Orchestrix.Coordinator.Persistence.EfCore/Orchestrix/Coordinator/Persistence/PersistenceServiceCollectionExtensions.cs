using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Orchestrix.Coordinator.Persistence.EfCore;
using Orchestrix.Coordinator.Persistence.EfCore.Stores;

namespace Orchestrix.Coordinator.Persistence;

/// <summary>
/// Extension methods for configuring persistence services.
/// </summary>
public static class PersistenceServiceCollectionExtensions
{
    /// <summary>
    /// Configures the coordinator to use Entity Framework Core for persistence.
    /// </summary>
    /// <param name="builder">The persistence configuration builder.</param>
    /// <param name="configure">Delegate to configure the DbContext options.</param>
    /// <returns>The persistence configuration builder.</returns>
    public static IPersistenceConfigurationBuilder UseEfCore(this IPersistenceConfigurationBuilder builder, Action<DbContextOptionsBuilder>? configure = null)
    {
        builder.Services.AddDbContext<CoordinatorDbContext>(options =>
        {
            if (configure != null)
            {
                configure(options);
            }
            else
            {
                // Default to in-memory if not configured? No, better warn or fail.
                // Or provider Npgsql via overload.
            }
        });

        builder.Services.TryAddScoped<IJobStore, JobStore>();
        builder.Services.TryAddScoped<IWorkerStore, WorkerStore>();
        builder.Services.TryAddScoped<ICoordinatorNodeStore, CoordinatorNodeStore>();
        builder.Services.TryAddScoped<ICronScheduleStore, CronScheduleStore>();
        builder.Services.TryAddScoped<IDeadLetterStore, DeadLetterStore>();
        builder.Services.TryAddScoped<IJobHistoryStore, JobHistoryStore>();
        builder.Services.TryAddScoped<ILogStore, LogStore>();

        return builder;
    }
}
