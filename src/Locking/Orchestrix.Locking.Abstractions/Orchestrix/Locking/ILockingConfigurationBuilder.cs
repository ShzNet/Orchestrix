using Microsoft.Extensions.DependencyInjection;

namespace Orchestrix.Locking;

/// <summary>
/// Builder interface for configuring locking services.
/// Extension methods should be defined in locking implementation packages (e.g., UseRedis in Orchestrix.Locking.Redis).
/// </summary>
public interface ILockingConfigurationBuilder
{
    /// <summary>
    /// Gets the service collection.
    /// </summary>
    IServiceCollection Services { get; }
}
