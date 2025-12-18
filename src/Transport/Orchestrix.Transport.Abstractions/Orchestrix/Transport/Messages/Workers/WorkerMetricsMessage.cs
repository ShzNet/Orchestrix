namespace Orchestrix.Transport.Messages.Workers;

/// <summary>
/// Message for worker performance metrics.
/// </summary>
public class WorkerMetricsMessage
{
    public string WorkerId { get; set; } = string.Empty;
    public double CpuUsage { get; set; }
    public long MemoryUsageMB { get; set; }
    public int ActiveJobs { get; set; }
    public int CompletedJobs { get; set; }
    public int FailedJobs { get; set; }
    public DateTimeOffset Timestamp { get; set; }
}
