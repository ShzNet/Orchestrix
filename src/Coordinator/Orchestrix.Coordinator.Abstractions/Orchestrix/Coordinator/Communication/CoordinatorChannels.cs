using Microsoft.Extensions.Options;
using Orchestrix.Transport;

namespace Orchestrix.Coordinator.Communication;

/// <summary>
/// Defines coordinator-specific channel names.
/// Follows the same pattern as TransportChannels.
/// </summary>
public class CoordinatorChannels
{
    private readonly string _prefix;

    /// <summary>
    /// Initializes a new instance with options from TransportOptions.
    /// </summary>
    public CoordinatorChannels(IOptions<TransportOptions> options)
    {
        _prefix = options.Value.ChannelPrefix;
    }

   

    /// <summary>
    /// Channel for coordinator node metrics and heartbeats.
    /// Used by Leader to track node health and by Control Panel for realtime monitoring.
    /// </summary>
    public string CoordinatorMetrics => $"{_prefix}:coordinator:metrics";

    /// <summary>
    /// Channel for job dispatched events (broadcast to all followers).
    /// Followers race to claim ownership of dispatched jobs.
    /// </summary>
    public string JobDispatched => $"{_prefix}:job:dispatched";

    /// <summary>
    /// Channel for job handoff requests during scale down or crash recovery.
    /// </summary>
    public string JobHandoff => $"{_prefix}:job:handoff";
}
