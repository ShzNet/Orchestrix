namespace Orchestrix.Transport.Messages.Jobs;

/// <summary>
/// Message to cancel a running job.
/// </summary>
public class JobCancelMessage
{
    public Guid JobId { get; set; }
    public string? Reason { get; set; }
}
