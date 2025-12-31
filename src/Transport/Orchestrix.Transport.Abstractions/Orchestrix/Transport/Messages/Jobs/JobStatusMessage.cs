namespace Orchestrix.Transport.Messages.Jobs;

using Orchestrix.Enums;

/// <summary>
/// Message for job status updates and completion results.
/// </summary>
public class JobStatusMessage
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
    /// The current status of the job.
    /// </summary>
    public JobStatus Status { get; set; }

    /// <summary>
    /// The result of the job execution (if any).
    /// </summary>
    public string? Result { get; set; }

    /// <summary>
    /// The error message if the job failed.
    /// </summary>
    public string? Error { get; set; }

    /// <summary>
    /// The timestamp when the job completed.
    /// </summary>
    public DateTimeOffset? CompletedAt { get; set; }
}
