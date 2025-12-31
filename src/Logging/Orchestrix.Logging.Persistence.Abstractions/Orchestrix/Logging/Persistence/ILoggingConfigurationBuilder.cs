using Microsoft.Extensions.DependencyInjection;

namespace Orchestrix.Logging.Persistence;

/// <summary>
/// A builder for configuring logging services.
/// </summary>
public interface ILoggingConfigurationBuilder
{
    /// <summary>
    /// Gets the service collection.
    /// </summary>
    IServiceCollection Services { get; }
}
