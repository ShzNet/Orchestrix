using Microsoft.Extensions.DependencyInjection;

namespace Orchestrix.Transport;

/// <summary>
/// Builder interface for configuring transport services.
/// </summary>
public interface ITransportBuilder
{
    /// <summary>
    /// Gets the service collection for registering transport dependencies.
    /// </summary>
    IServiceCollection Services { get; }
}
