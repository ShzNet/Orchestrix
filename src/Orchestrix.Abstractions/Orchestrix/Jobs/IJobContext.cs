namespace Orchestrix.Jobs;

using Orchestrix.Enums;

/// <summary>
/// Provides context and utilities for job execution.
/// </summary>
public interface IJobContext
{
    /// <summary>
    /// Gets the unique identifier of the job.
    /// </summary>
    Guid JobId { get; }

    /// <summary>
    /// Gets the unique identifier of the job execution history entry.
    /// </summary>
    Guid HistoryId { get; }

    /// <summary>
    /// Gets the name/type of the job.
    /// </summary>
    string JobName { get; }

    /// <summary>
    /// Gets the queue name where the job was dispatched.
    /// </summary>
    string Queue { get; }

    /// <summary>
    /// Gets the current retry attempt count (0 for first attempt).
    /// </summary>
    int RetryCount { get; }

    /// <summary>
    /// Gets the maximum number of retry attempts allowed.
    /// </summary>
    int MaxRetries { get; }

    /// <summary>
    /// Gets the correlation ID for tracking related operations.
    /// </summary>
    string? CorrelationId { get; }

    /// <summary>
    /// Gets the cancellation token for the job execution.
    /// </summary>
    CancellationToken CancellationToken { get; }

    /// <summary>
    /// Logs a message during job execution.
    /// </summary>
    /// <param name="message">The log message.</param>
    /// <param name="level">The log level (default: Information).</param>
    Task LogAsync(string message, LogLevel level = LogLevel.Information);

    /// <summary>
    /// Updates the job execution progress.
    /// </summary>
    /// <param name="percentage">Progress percentage (0-100).</param>
    /// <param name="message">Optional progress message.</param>
    Task UpdateProgressAsync(int percentage, string? message = null);
}
