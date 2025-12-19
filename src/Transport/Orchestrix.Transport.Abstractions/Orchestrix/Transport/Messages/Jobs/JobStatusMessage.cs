namespace Orchestrix.Transport.Messages.Jobs;

using Orchestrix.Enums;

/// <summary>
/// Message for job status updates and completion results.
/// </summary>
public class JobStatusMessage
{
    public Guid JobId { get; set; }
    public Guid ExecutionId { get; set; }
    public JobStatus Status { get; set; }
    public string? Result { get; set; }
    public string? Error { get; set; }
    public DateTimeOffset? CompletedAt { get; set; }
}
