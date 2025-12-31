namespace Orchestrix.Coordinator.Services.Monitoring;

/// <summary>
/// In-memory implementation of <see cref="IJobStatsCache"/>.
/// Thread-safe.
/// </summary>
public class JobStatsCache : IJobStatsCache
{
    private int _monitoredJobCount;
    private int _queuedJobCount;

    /// <inheritdoc />
    public int MonitoredJobCount => _monitoredJobCount;

    /// <inheritdoc />
    public int QueuedJobCount => _queuedJobCount;

    /// <inheritdoc />
    public void UpdateMonitoredJobCount(int delta)
    {
        Interlocked.Add(ref _monitoredJobCount, delta);
    }

    /// <inheritdoc />
    public void SetMonitoredJobCount(int count)
    {
        Interlocked.Exchange(ref _monitoredJobCount, count);
    }

    /// <inheritdoc />
    public void SetQueuedJobCount(int count)
    {
        Interlocked.Exchange(ref _queuedJobCount, count);
    }
}
