namespace Orchestrix.Coordinator.Services.Monitoring;

/// <summary>
/// Maintains in-memory statistics about jobs for this node.
/// Updated by job processing components to avoid frequent DB queries.
/// </summary>
public interface IJobStatsCache
{
    /// <summary>
    /// Gets the number of jobs currently active/monitored by this node.
    /// </summary>
    int MonitoredJobCount { get; }

    /// <summary>
    /// Gets the number of queued jobs awaiting distribution (known by this node).
    /// </summary>
    int QueuedJobCount { get; }

    /// <summary>
    /// Updates the monitored job count (delta).
    /// </summary>
    void UpdateMonitoredJobCount(int delta);
    
    /// <summary>
    /// Sets the absolute monitored job count.
    /// </summary>
    void SetMonitoredJobCount(int count);

    /// <summary>
    /// Sets the queued job count (e.g. from periodic poll by Leader).
    /// </summary>
    void SetQueuedJobCount(int count);
}
