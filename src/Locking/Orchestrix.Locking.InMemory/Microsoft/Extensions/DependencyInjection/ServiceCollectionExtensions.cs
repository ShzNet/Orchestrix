using Orchestrix.Locking;
using Orchestrix.Locking.InMemory;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods for registering in-memory locking services.
/// </summary>
public static class InMemoryLockingServiceCollectionExtensions
{
    /// <summary>
    /// Configures in-memory distributed locking (for development/testing).
    /// </summary>
    /// <param name="builder">The locking builder.</param>
    /// <returns>The locking builder for chaining.</returns>
    public static ILockingBuilder UseInMemory(this ILockingBuilder builder)
    {
        // Register lock provider
        builder.Services.AddSingleton<IDistributedLockProvider, InMemoryLockProvider>();

        return builder;
    }
}
