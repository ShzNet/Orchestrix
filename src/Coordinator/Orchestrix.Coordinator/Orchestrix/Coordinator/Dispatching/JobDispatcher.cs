using Microsoft.Extensions.Logging;
using Orchestrix.Coordinator.Persistence;
using Orchestrix.Coordinator.Persistence.Entities;
using Orchestrix.Transport;
using Orchestrix.Transport.Messages.Jobs;

namespace Orchestrix.Coordinator.Dispatching;

/// <summary>
/// Implementation of job dispatcher.
/// </summary>
internal class JobDispatcher(
    IPublisher publisher,
    IJobStore jobStore,
    TransportChannels channels,
    ILogger<JobDispatcher> logger)
    : IJobDispatcher
{
    public async Task DispatchAsync(JobEntity job, CancellationToken cancellationToken = default)
    {
        try
        {
            // Publish JobDispatchMessage to job.dispatch.{queue} for workers
            var dispatchMessage = new JobDispatchMessage
            {
                JobId = job.Id,
                ExecutionId = Guid.NewGuid(), // Create new execution ID
                JobType = job.JobType,
                Arguments = job.ArgumentsJson,
                RetryCount = job.RetryCount,
                MaxRetries = job.MaxRetries,
                CorrelationId = job.CorrelationId
            };

            var dispatchChannel = channels.JobDispatch(job.Queue);
            await publisher.PublishAsync(dispatchChannel, dispatchMessage, cancellationToken);

            // Update job status to Dispatched
            await jobStore.MarkDispatchedAsync(
                job.Id,
                string.Empty, // WorkerId not known yet
                DateTimeOffset.UtcNow,
                cancellationToken);

            logger.LogInformation(
                "Dispatched job {JobId} (execution {ExecutionId}) to queue {Queue}",
                job.Id, dispatchMessage.ExecutionId, job.Queue);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error dispatching job {JobId}", job.Id);
            throw;
        }
    }
}
