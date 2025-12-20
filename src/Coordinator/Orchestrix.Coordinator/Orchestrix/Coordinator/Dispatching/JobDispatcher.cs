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
    Orchestrix.Coordinator.Ownership.JobAssignmentPublisher assignmentPublisher,
    ILogger<JobDispatcher> logger)
    : IJobDispatcher
{
    public async Task DispatchAsync(JobEntity job, CancellationToken cancellationToken = default)
    {
        try
        {
            var executionId = Guid.NewGuid(); // Create new execution ID

            // 1. Publish to job.dispatch.{queue} for WORKERS
            //    Workers consume with consumer group "workers"
            var dispatchMessage = new JobDispatchMessage
            {
                JobId = job.Id,
                ExecutionId = executionId,
                JobType = job.JobType,
                Arguments = job.ArgumentsJson,
                RetryCount = job.RetryCount,
                MaxRetries = job.MaxRetries,
                CorrelationId = job.CorrelationId
            };

            var dispatchChannel = channels.JobDispatch(job.Queue);
            await publisher.PublishAsync(dispatchChannel, dispatchMessage, cancellationToken);

            // 2. Update job status to Dispatched
            await jobStore.MarkDispatchedAsync(
                job.Id,
                string.Empty, // WorkerId not known yet
                DateTimeOffset.UtcNow,
                cancellationToken);

            // 3. Publish to job.dispatched for FOLLOWERS
            //    Followers consume with consumer group "followers" to race-to-claim ownership
            await assignmentPublisher.PublishJobAssignedAsync(
                job.Id,
                executionId,
                job.Queue,
                cancellationToken);

            logger.LogInformation(
                "Dispatched job {JobId} (execution {ExecutionId}) to queue {Queue}",
                job.Id, executionId, job.Queue);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error dispatching job {JobId}", job.Id);
            throw;
        }
    }
}
