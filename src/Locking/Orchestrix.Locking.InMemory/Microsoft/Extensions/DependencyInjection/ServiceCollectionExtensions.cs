namespace Microsoft.Extensions.DependencyInjection;

using Orchestrix.Locking;
using Orchestrix.Locking.InMemory;

/// <summary>
/// Extension methods for registering in-memory locking services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds in-memory distributed locking services.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddInMemoryLocking(this IServiceCollection services)
    {
        services.AddSingleton<IDistributedLockProvider, InMemoryLockProvider>();
        return services;
    }

    /// <summary>
    /// Configures in-memory distributed locking.
    /// </summary>
    /// <param name="options">The locking options.</param>
    /// <returns>The locking options for chaining.</returns>
    public static LockingOptions UseInMemory(this LockingOptions options)
    {
        options.Services.AddSingleton<IDistributedLockProvider, InMemoryLockProvider>();
        return options;
    }
}
