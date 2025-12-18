namespace Orchestrix.Locking;

/// <summary>
/// Options for configuring distributed locks.
/// </summary>
public class DistributedLockOptions
{
    /// <summary>
    /// Gets or sets the default time-to-live for locks.
    /// Default is 30 seconds.
    /// </summary>
    public TimeSpan DefaultTtl { get; set; } = TimeSpan.FromSeconds(30);
}
