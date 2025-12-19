namespace Orchestrix.Transport.Messages.Jobs;

using Orchestrix.Enums;

/// <summary>
/// Message for job log entries.
/// </summary>
public class JobLogMessage
{
    public Guid JobId { get; set; }
    public Guid ExecutionId { get; set; }
    public LogLevel Level { get; set; }
    public string Message { get; set; } = string.Empty;
}
