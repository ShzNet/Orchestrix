namespace Orchestrix.Transport.Messages.Jobs;

using Orchestrix.Enums;

/// <summary>
/// Message for job log entries.
/// </summary>
public class JobLogMessage
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
    /// The log level of the entry.
    /// </summary>
    public LogLevel Level { get; set; }

    /// <summary>
    /// The log message content.
    /// </summary>
    public string Message { get; set; } = string.Empty;
}
