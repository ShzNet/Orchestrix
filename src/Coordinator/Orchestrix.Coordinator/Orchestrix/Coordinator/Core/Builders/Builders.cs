using Microsoft.Extensions.DependencyInjection;
using Orchestrix.Persistence;
using Orchestrix.Locking;
using Orchestrix.Transport;

namespace Orchestrix.Coordinator.Core.Builders;

internal class TransportConfigurationBuilder(IServiceCollection services) : ITransportConfigurationBuilder
{
    public IServiceCollection Services { get; } = services;
}

internal class LockingConfigurationBuilder(IServiceCollection services) : ILockingConfigurationBuilder
{
    public IServiceCollection Services { get; } = services;
}

internal class PersistenceConfigurationBuilder(IServiceCollection services) : IPersistenceConfigurationBuilder
{
    public IServiceCollection Services { get; } = services;
}