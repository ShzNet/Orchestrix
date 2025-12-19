using Microsoft.Extensions.DependencyInjection;

namespace Orchestrix.Coordinator;

/// <summary>
/// Implementation of ICoordinatorBuilder for fluent configuration.
/// </summary>
internal class CoordinatorBuilder : ICoordinatorBuilder
{
    public CoordinatorBuilder(IServiceCollection services)
    {
        Services = services;
    }

    public IServiceCollection Services { get; }
}
