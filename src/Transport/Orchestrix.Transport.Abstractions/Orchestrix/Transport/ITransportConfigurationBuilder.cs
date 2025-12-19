using Microsoft.Extensions.DependencyInjection;

namespace Orchestrix.Transport;

/// <summary>
/// Builder interface for configuring transport services.
/// Extension methods should be defined in transport implementation packages (e.g., UseRedis in Orchestrix.Transport.Redis).
/// </summary>
public interface ITransportConfigurationBuilder
{
    /// <summary>
    /// Gets the service collection.
    /// </summary>
    IServiceCollection Services { get; }
}
