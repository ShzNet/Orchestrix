namespace Orchestrix.Transport.Redis;

/// <summary>
/// Redis-specific transport configuration options.
/// </summary>
public class RedisTransportOptions
{
    /// <summary>
    /// Maximum number of entries to keep in a stream.
    /// Uses approximate trimming for performance.
    /// Default: 10000.
    /// </summary>
    public int MaxStreamLength { get; set; } = 10000;
}
