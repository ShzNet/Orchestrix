using Microsoft.Extensions.DependencyInjection;

namespace Orchestrix.Persistence;

/// <summary>
/// Fluent builder interface for configuring Coordinator persistence.
/// </summary>
public interface IPersistenceConfigurationBuilder
{
    /// <summary>
    /// Gets the service collection.
    /// </summary>
    IServiceCollection Services { get; }
}
