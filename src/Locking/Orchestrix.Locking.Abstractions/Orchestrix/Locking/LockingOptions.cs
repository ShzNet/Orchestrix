namespace Orchestrix.Locking;

using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Options for configuring distributed locking.
/// Used in nested configuration pattern.
/// </summary>
public class LockingOptions
{
    /// <summary>
    /// Gets the service collection for registering locking services.
    /// Used by extension methods (UseInMemory, UseRedis, etc.)
    /// </summary>
    public IServiceCollection Services { get; internal set; } = null!;

    /// <summary>
    /// Gets or sets the default time-to-live for locks.
    /// Default is 30 seconds.
    /// </summary>
    public TimeSpan DefaultTtl { get; set; } = TimeSpan.FromSeconds(30);
}
