using Microsoft.Extensions.DependencyInjection;

namespace Orchestrix.Locking;

/// <summary>
/// Builder interface for configuring distributed locking services.
/// </summary>
public interface ILockingBuilder
{
    /// <summary>
    /// Gets the service collection for registering locking dependencies.
    /// </summary>
    IServiceCollection Services { get; }
}
