using Microsoft.Extensions.Logging;
using Orchestrix.Transport;

namespace Orchestrix.Coordinator.Ownership;

/// <summary>
/// Service for subscribing to job-specific channels.
/// </summary>
internal class JobChannelSubscriber(
    ISubscriber subscriber,
    TransportChannels channels,
    JobEventProcessor eventProcessor,
    ILogger<JobChannelSubscriber> logger)
{
    /// <summary>
    /// Subscribes to job-specific channels (status, logs).
    /// </summary>
    public async Task SubscribeToJobChannelsAsync(Guid jobId, Guid executionId)
    {
        try
        {
            // Subscribe to job.{executionId}.status
            var statusChannel = channels.JobStatus(executionId);
            await subscriber.SubscribeAsync<object>(
                channel: statusChannel,
                handler: async (msg) =>
                {
                    await HandleJobStatusAsync(jobId, msg);
                    return true; // Keep subscription
                });

            logger.LogDebug("Subscribed to {Channel}", statusChannel);

            // Subscribe to job.{executionId}.log
            var logChannel = channels.JobLog(executionId);
            await subscriber.SubscribeAsync<object>(
                channel: logChannel,
                handler: async (msg) =>
                {
                    await HandleJobLogAsync(jobId, msg);
                    return true; // Keep subscription
                });

            logger.LogDebug("Subscribed to {Channel}", logChannel);
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                "Error subscribing to job channels for JobId={JobId}",
                jobId);
            throw;
        }
    }

    /// <summary>
    /// Unsubscribes from job-specific channels.
    /// </summary>
    public async Task UnsubscribeFromJobChannelsAsync(Guid jobId)
    {
        try
        {
            var statusChannel = channels.JobStatus(jobId);
            await subscriber.UnsubscribeAsync(statusChannel);

            logger.LogDebug("Unsubscribed from {Channel}", statusChannel);

            // Unsubscribe from log channel
            var logChannel = channels.JobLog(jobId);
            await subscriber.UnsubscribeAsync(logChannel);

            logger.LogDebug("Unsubscribed from {Channel}", logChannel);
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                "Error unsubscribing from job channels for JobId={JobId}",
                jobId);
        }
    }

    private async Task HandleJobStatusAsync(Guid jobId, object message)
    {
        // Delegate to JobEventProcessor
        if (message is Transport.Messages.Jobs.JobStatusMessage statusMsg)
        {
            await eventProcessor.ProcessStatusUpdateAsync(statusMsg);
        }
        else
        {
            logger.LogWarning(
                "Received unexpected message type {Type} on job status channel for JobId={JobId}",
                message.GetType().Name, jobId);
        }
    }

    private async Task HandleJobLogAsync(Guid jobId, object message)
    {
        // Delegate to JobEventProcessor
        if (message is Transport.Messages.Jobs.JobLogMessage logMsg)
        {
            await eventProcessor.ProcessLogEntryAsync(logMsg);
        }
        else
        {
            logger.LogWarning(
                "Received unexpected message type {Type} on job log channel for JobId={JobId}",
                message.GetType().Name, jobId);
        }
    }
}
