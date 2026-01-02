using Microsoft.Extensions.Logging;
using Orchestrix.Persistence;
using Orchestrix.Persistence.Entities;
using Orchestrix.Transport;
using Orchestrix.Transport.Messages.Jobs;

namespace Orchestrix.Coordinator.Services.Dispatching;

/// <summary>
/// Implementation of job dispatching to workers via transport.
/// </summary>
public class JobDispatcher : IJobDispatcher
{
    private readonly IPublisher _publisher;
    private readonly TransportChannels _channels;
    private readonly IJobStore _jobStore;
    private readonly ILogger<JobDispatcher> _logger;

    /// <summary>
    /// Initializes a new instance of <see cref="JobDispatcher"/>.
    /// </summary>
    /// <param name="publisher">The message publisher.</param>
    /// <param name="channels">The transport channels.</param>
    /// <param name="jobStore">The job store.</param>
    /// <param name="logger">The logger.</param>
    public JobDispatcher(
        IPublisher publisher,
        TransportChannels channels,
        IJobStore jobStore,
        ILogger<JobDispatcher> logger)
    {
        _publisher = publisher;
        _channels = channels;
        _jobStore = jobStore;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<Guid> DispatchAsync(JobEntity job, CancellationToken cancellationToken = default)
    {
        var executionId = Guid.NewGuid();
        var now = DateTimeOffset.UtcNow;

        // Create dispatch message
        var message = new JobDispatchMessage
        {
            JobId = job.Id,
            ExecutionId = executionId,
            JobType = job.JobType,
            Arguments = job.ArgumentsJson,
            RetryCount = job.RetryCount,
            MaxRetries = job.MaxRetries,
            CorrelationId = job.CorrelationId
        };

        // Publish to queue-specific channel
        var channel = _channels.JobDispatch(job.Queue);
        await _publisher.PublishAsync(channel, message, cancellationToken);

        _logger.LogInformation(
            "[JobDispatcher] Dispatched job {JobId} (ExecutionId: {ExecutionId}) to channel {Channel}",
            job.Id, executionId, channel);

        // Update job status to Dispatched
        // Note: WorkerId will be set when worker picks up the job
        await _jobStore.MarkDispatchedAsync(job.Id, string.Empty, now, cancellationToken);

        return executionId;
    }
}
