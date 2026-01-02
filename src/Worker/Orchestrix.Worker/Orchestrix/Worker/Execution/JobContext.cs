using Microsoft.Extensions.Logging;
using Orchestrix.Enums;
using Orchestrix.Jobs;
using Orchestrix.Transport;
using Orchestrix.Transport.Messages.Jobs;

namespace Orchestrix.Worker.Execution;

/// <summary>
/// Implementation of IJobContext for job execution.
/// </summary>
public class JobContext : IJobContext
{
    private readonly IPublisher _publisher;
    private readonly TransportChannels _channels;
    private readonly ILogger _logger;

    /// <summary>
    /// Initializes a new instance of <see cref="JobContext"/>.
    /// </summary>
    /// <param name="message">The job dispatch message.</param>
    /// <param name="queue">The queue name.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <param name="publisher">The message publisher.</param>
    /// <param name="channels">The transport channels.</param>
    /// <param name="logger">The logger.</param>
    public JobContext(
        JobDispatchMessage message,
        string queue,
        CancellationToken cancellationToken,
        IPublisher publisher,
        TransportChannels channels,
        ILogger logger)
    {
        Message = message;
        JobId = message.JobId;
        HistoryId = message.ExecutionId;
        JobName = message.JobType;
        Queue = queue;
        RetryCount = message.RetryCount;
        MaxRetries = message.MaxRetries;
        CorrelationId = message.CorrelationId;
        CancellationToken = cancellationToken;
        _publisher = publisher;
        _channels = channels;
        _logger = logger;
    }

    /// <summary>
    /// Gets the original dispatch message (internal use).
    /// </summary>
    internal JobDispatchMessage Message { get; }

    /// <inheritdoc />
    public Guid JobId { get; }

    /// <inheritdoc />
    public Guid HistoryId { get; }

    /// <inheritdoc />
    public string JobName { get; }

    /// <inheritdoc />
    public string Queue { get; }

    /// <inheritdoc />
    public int RetryCount { get; }

    /// <inheritdoc />
    public int MaxRetries { get; }

    /// <inheritdoc />
    public string? CorrelationId { get; }

    /// <inheritdoc />
    public CancellationToken CancellationToken { get; }

    /// <inheritdoc />
    public async Task LogAsync(string message, Orchestrix.Enums.LogLevel level = Orchestrix.Enums.LogLevel.Information)
    {
        var logMessage = new JobLogMessage
        {
            JobId = JobId,
            ExecutionId = HistoryId,
            Level = level,
            Message = message
        };

        var channel = _channels.JobLog(HistoryId);
        await _publisher.PublishAsync(channel, logMessage, CancellationToken);

        // Also log locally
        _logger.Log(MapLogLevel(level), "[Job {JobId}] {Message}", JobId, message);
    }

    /// <inheritdoc />
    public async Task UpdateProgressAsync(int percentage, string? message = null)
    {
        // Note: JobStatusMessage doesn't have Progress property, 
        // we log progress locally and send a Running status
        var statusMessage = new JobStatusMessage
        {
            JobId = JobId,
            ExecutionId = HistoryId,
            Status = JobStatus.Running,
            Result = $"Progress: {percentage}%{(message != null ? $" - {message}" : "")}"
        };

        var channel = _channels.JobStatus(HistoryId);
        await _publisher.PublishAsync(channel, statusMessage, CancellationToken);

        _logger.LogDebug("[Job {JobId}] Progress: {Percentage}% - {Message}", JobId, percentage, message);
    }

    private static Microsoft.Extensions.Logging.LogLevel MapLogLevel(Orchestrix.Enums.LogLevel level)
    {
        return level switch
        {
            Orchestrix.Enums.LogLevel.Debug => Microsoft.Extensions.Logging.LogLevel.Debug,
            Orchestrix.Enums.LogLevel.Information => Microsoft.Extensions.Logging.LogLevel.Information,
            Orchestrix.Enums.LogLevel.Warning => Microsoft.Extensions.Logging.LogLevel.Warning,
            Orchestrix.Enums.LogLevel.Error => Microsoft.Extensions.Logging.LogLevel.Error,
            _ => Microsoft.Extensions.Logging.LogLevel.Information
        };
    }
}
