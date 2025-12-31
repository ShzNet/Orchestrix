namespace Orchestrix.Transport.Messages.Jobs;

/// <summary>
/// Message to cancel a running job.
/// </summary>
public class JobCancelMessage
{
    /// <summary>
    /// The unique identifier of the job to cancel.
    /// </summary>
    public Guid JobId { get; set; }

    /// <summary>
    /// The reason for cancellation.
    /// </summary>
    public string? Reason { get; set; }
}
