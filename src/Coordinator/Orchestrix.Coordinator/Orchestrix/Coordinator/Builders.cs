using Microsoft.Extensions.DependencyInjection;
using Orchestrix.Locking;
using Orchestrix.Transport;

namespace Orchestrix.Coordinator;

/// <summary>
/// Internal implementation of ITransportConfigurationBuilder.
/// </summary>
internal class TransportConfigurationBuilder(IServiceCollection services) : ITransportConfigurationBuilder
{
    public IServiceCollection Services { get; } = services;
}

/// <summary>
/// Internal implementation of ILockingConfigurationBuilder.
/// </summary>
internal class LockingConfigurationBuilder(IServiceCollection services) : ILockingConfigurationBuilder
{
    public IServiceCollection Services { get; } = services;
}

/// <summary>
/// Internal implementation of IPersistenceConfigurationBuilder.
/// </summary>
internal class PersistenceConfigurationBuilder(IServiceCollection services) : IPersistenceConfigurationBuilder
{
    public IServiceCollection Services { get; } = services;
}
