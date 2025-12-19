using Orchestrix.Locking;
using Orchestrix.Locking.InMemory;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods for configuring in-memory locking.
/// </summary>
public static class LockingServiceCollectionExtensions
{
    /// <summary>
    /// Configures in-memory distributed lock provider (for development/testing only).
    /// </summary>
    /// <param name="builder">The locking configuration builder.</param>
    /// <returns>The locking configuration builder for chaining.</returns>
    public static ILockingConfigurationBuilder UseInMemory(this ILockingConfigurationBuilder builder)
    {
        // Register lock provider
        builder.Services.AddSingleton<IDistributedLockProvider, InMemoryLockProvider>();

        return builder;
    }
}
