using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Orchestrix.Coordinator.Caching;
using Orchestrix.Coordinator.Communication;
using Orchestrix.Coordinator.Communication.Cluster;
using Orchestrix.Coordinator.Persistence;
using Orchestrix.Transport;

namespace Orchestrix.Coordinator.Ownership;

/// <summary>
/// Background service that subscribes to job assignments and claims ownership.
/// </summary>
internal class JobAssignmentSubscriber(
    ISubscriber subscriber,
    IJobStore jobStore,
    IJobOwnershipRegistry ownershipRegistry,
    JobChannelSubscriber channelSubscriber,
    CoordinatorChannels channels,
    ICacheInvalidator cacheInvalidator,
    IOptions<CoordinatorOptions> options,
    ILogger<JobAssignmentSubscriber> logger)
    : BackgroundService
{
    private readonly string _nodeId = options.Value.NodeId;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("JobAssignmentSubscriber starting for node {NodeId}", _nodeId);

        try
        {
            // Subscribe to job.dispatched with consumer group "followers"
            await subscriber.SubscribeWithGroupAsync<JobDispatchedEvent>(
                channel: channels.JobDispatched,
                groupName: "followers",
                consumerName: _nodeId,
                handler: async (msg) => { await HandleJobDispatchedAsync(msg); return true; },
                cancellationToken: stoppingToken);

            logger.LogInformation("Subscribed to {Channel} with consumer group 'followers'", channels.JobDispatched);

            // Keep running until cancellation
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
        catch (OperationCanceledException)
        {
            logger.LogInformation("JobAssignmentSubscriber stopping for node {NodeId}", _nodeId);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Fatal error in JobAssignmentSubscriber");
            throw;
        }
    }

    private async Task HandleJobDispatchedAsync(JobDispatchedEvent message)
    {
        try
        {
            logger.LogDebug(
                "Received job assignment: JobId={JobId}, ExecutionId={ExecutionId}, Queue={Queue}",
                message.JobId, message.ExecutionId, message.Queue);

            // Race to claim ownership in database
            var claimed = await jobStore.TryClaimJobAsync(message.JobId, _nodeId);

            if (!claimed)
            {
                logger.LogDebug(
                    "Job {JobId} already claimed by another follower",
                    message.JobId);
                return;
            }

            // Register ownership in memory
            await ownershipRegistry.ClaimAsync(
                message.JobId,
                _nodeId,
                message.ExecutionId);

            // Invalidate cache after claiming (job.FollowerNodeId changed in DB)
            await cacheInvalidator.InvalidateJobAsync(message.JobId);

            logger.LogInformation(
                "Claimed ownership of job {JobId} (execution {ExecutionId})",
                message.JobId, message.ExecutionId);

            // Subscribe to job-specific channels
            await channelSubscriber.SubscribeToJobChannelsAsync(message.JobId, message.ExecutionId);
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                "Error handling job assignment for JobId={JobId}",
                message.JobId);
        }
    }
}
