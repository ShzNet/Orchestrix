namespace Orchestrix.Transport.Messages.Jobs;

/// <summary>
/// Message to dispatch a job to a worker.
/// </summary>
public class JobDispatchMessage
{
    /// <summary>
    /// The unique identifier of the job.
    /// </summary>
    public Guid JobId { get; set; }

    /// <summary>
    /// The unique identifier of the execution attempt.
    /// </summary>
    public Guid ExecutionId { get; set; }

    /// <summary>
    /// The type of the job.
    /// </summary>
    public string JobType { get; set; } = string.Empty;

    /// <summary>
    /// The arguments for the job execution, serialized as JSON.
    /// </summary>
    public string Arguments { get; set; } = string.Empty;

    /// <summary>
    /// The current retry count for the job.
    /// </summary>
    public int RetryCount { get; set; }

    /// <summary>
    /// The maximum number of retries allowed for the job.
    /// </summary>
    public int MaxRetries { get; set; }

    /// <summary>
    /// The correlation ID for tracing the job execution flow.
    /// </summary>
    public string? CorrelationId { get; set; }
}
