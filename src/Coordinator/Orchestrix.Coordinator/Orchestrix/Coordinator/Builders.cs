using Microsoft.Extensions.DependencyInjection;
using Orchestrix.Locking;
using Orchestrix.Transport;

namespace Orchestrix.Coordinator;

/// <summary>
/// Internal implementation of ITransportBuilder.
/// </summary>
internal class TransportBuilder : ITransportBuilder
{
    public IServiceCollection Services { get; }

    public TransportBuilder(IServiceCollection services)
    {
        Services = services;
    }
}

/// <summary>
/// Internal implementation of ILockingBuilder.
/// </summary>
internal class LockingBuilder : ILockingBuilder
{
    public IServiceCollection Services { get; }

    public LockingBuilder(IServiceCollection services)
    {
        Services = services;
    }
}

/// <summary>
/// Internal implementation of IPersistenceBuilder.
/// </summary>
internal class PersistenceBuilder : IPersistenceBuilder
{
    public IServiceCollection Services { get; }

    public PersistenceBuilder(IServiceCollection services)
    {
        Services = services;
    }
}
