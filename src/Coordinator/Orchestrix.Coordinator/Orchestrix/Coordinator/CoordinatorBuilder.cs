using Microsoft.Extensions.DependencyInjection;

namespace Orchestrix.Coordinator;

/// <summary>
/// Implementation of ICoordinatorConfigurationBuilder for fluent configuration.
/// </summary>
internal class CoordinatorBuilder : ICoordinatorConfigurationBuilder
{
    public CoordinatorBuilder(IServiceCollection services)
    {
        Services = services;
    }

    public IServiceCollection Services { get; }
}
