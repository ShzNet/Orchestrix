using Microsoft.Extensions.DependencyInjection;

namespace Orchestrix.Coordinator;

/// <summary>
/// Fluent builder interface for configuring Coordinator services.
/// Provides extension points for Transport, Locking, and Persistence configuration.
/// </summary>
/// <remarks>
/// Extension methods for this interface should be defined in respective packages:
/// - UseRedisTransport() in Orchestrix.Transport.Redis
/// - UseRedisLocking() in Orchestrix.Locking.Redis
/// - UseEfCorePersistence() in Orchestrix.Persistence.EfCore
/// </remarks>
public interface ICoordinatorConfigurationBuilder
{
    /// <summary>
    /// Gets the service collection for registering dependencies.
    /// </summary>
    IServiceCollection Services { get; }
}
