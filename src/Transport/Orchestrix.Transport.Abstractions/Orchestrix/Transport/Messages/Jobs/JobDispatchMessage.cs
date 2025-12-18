namespace Orchestrix.Transport.Messages.Jobs;

/// <summary>
/// Message to dispatch a job to a worker.
/// </summary>
public class JobDispatchMessage
{
    public Guid JobId { get; set; }
    public Guid HistoryId { get; set; }
    public string JobType { get; set; } = string.Empty;
    public string Arguments { get; set; } = string.Empty;
    public int RetryCount { get; set; }
    public int MaxRetries { get; set; }
    public string? CorrelationId { get; set; }
}
