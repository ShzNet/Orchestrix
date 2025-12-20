using Orchestrix.Coordinator.Caching;
using Orchestrix.Coordinator.Communication;
using Orchestrix.Transport;

namespace Orchestrix.Coordinator.Ownership;

/// <summary>
/// Publishes job assignment events to followers on separate queue.
/// </summary>
internal class JobAssignmentPublisher(
    IPublisher publisher,
    CoordinatorChannels channels,
    ICacheInvalidator cacheInvalidator)
{
    public async Task PublishJobAssignedAsync(
        Guid jobId,
        Guid executionId,
        string queue,
        CancellationToken cancellationToken = default)
    {
        var message = new Communication.Cluster.JobDispatchedEvent
        {
            JobId = jobId,
            ExecutionId = executionId,
            Queue = queue,
            Timestamp = DateTimeOffset.UtcNow
        };

        // Publish to job.dispatched channel for followers
        // Followers consume with consumer group to race-to-claim ownership
        var channel = channels.JobDispatched;
        await publisher.PublishAsync(channel, message, cancellationToken);

        // Invalidate cache after assignment
        await cacheInvalidator.InvalidateJobAsync(jobId, cancellationToken);
        await cacheInvalidator.InvalidateJobsByQueueAsync(queue, cancellationToken);
    }
}
