using Orchestrix.Logging.Persistence.Entities;

namespace Orchestrix.Logging.Persistence;

/// <summary>
/// Repository interface for job log persistence operations.
/// </summary>
public interface ILogStore
{
    /// <summary>
    /// Appends a log entry for a job.
    /// </summary>
    Task AppendAsync(LogEntry logEntry, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all log entries for a specific job.
    /// </summary>
    Task<IReadOnlyList<LogEntry>> GetByJobIdAsync(Guid jobId, CancellationToken cancellationToken = default);
}
