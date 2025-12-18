namespace Orchestrix.Transport;

/// <summary>
/// Defines transport channel names for Worker-Coordinator communication.
/// </summary>
public class TransportChannels
{
    private readonly string _prefix;

    /// <summary>
    /// Initializes a new instance of the <see cref="TransportChannels"/> class.
    /// </summary>
    /// <param name="options">The transport options containing the channel prefix.</param>
    public TransportChannels(TransportOptions options)
    {
        _prefix = options.ChannelPrefix;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TransportChannels"/> class with a custom prefix.
    /// </summary>
    /// <param name="prefix">The channel prefix. Default is "orchestrix".</param>
    public TransportChannels(string prefix = "orchestrix")
    {
        _prefix = prefix;
    }

    /// <summary>
    /// Gets the channel for dispatching jobs to workers by queue (Coordinator → Worker).
    /// </summary>
    /// <param name="queueName">The queue name.</param>
    /// <returns>The channel name for the specified queue.</returns>
    public string JobDispatch(string queueName) => $"{_prefix}:job:dispatch:{queueName}";

    /// <summary>
    /// Gets the channel for job cancellation requests (Coordinator → Worker).
    /// </summary>
    public string JobCancel => $"{_prefix}:job:cancel";

    /// <summary>
    /// Gets the channel for job status updates by execution (Worker → Coordinator).
    /// Includes status changes and completion results.
    /// </summary>
    /// <param name="executionId">The execution/history ID.</param>
    /// <returns>The channel name for the specified execution.</returns>
    public string JobStatus(Guid executionId) => $"{_prefix}:job:{executionId}:status";

    /// <summary>
    /// Gets the channel for job log entries by execution (Worker → Coordinator).
    /// </summary>
    /// <param name="executionId">The execution/history ID.</param>
    /// <returns>The channel name for the specified execution.</returns>
    public string JobLog(Guid executionId) => $"{_prefix}:job:{executionId}:log";

    /// <summary>
    /// Gets the channel for worker join/registration (Worker → All Coordinators).
    /// Broadcast channel for all coordinators to know about new workers.
    /// </summary>
    public string WorkerJoin => $"{_prefix}:worker:join";

    /// <summary>
    /// Gets the channel for worker shutdown notifications by worker (Worker → Assigned Coordinator).
    /// Point-to-point channel to specific coordinator.
    /// </summary>
    /// <param name="workerId">The worker ID.</param>
    /// <returns>The channel name for the specified worker.</returns>
    public string WorkerShutdown(string workerId) => $"{_prefix}:worker:{workerId}:shutdown";

    /// <summary>
    /// Gets the channel for worker metrics by worker (Worker → Assigned Coordinator).
    /// Point-to-point channel. Also serves as heartbeat.
    /// </summary>
    /// <param name="workerId">The worker ID.</param>
    /// <returns>The channel name for the specified worker.</returns>
    public string WorkerMetrics(string workerId) => $"{_prefix}:worker:{workerId}:metrics";
}
