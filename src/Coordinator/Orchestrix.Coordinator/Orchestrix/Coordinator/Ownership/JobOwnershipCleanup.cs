using Microsoft.Extensions.Logging;

namespace Orchestrix.Coordinator.Ownership;

/// <summary>
/// Handles cleanup when job completes (unsubscribe channels, release ownership).
/// </summary>
internal class JobOwnershipCleanup(
    IJobOwnershipRegistry ownershipRegistry,
    JobChannelSubscriber channelSubscriber,
    ILogger<JobOwnershipCleanup> logger)
{
    /// <summary>
    /// Cleans up job ownership and subscriptions after job completion.
    /// </summary>
    public async Task CleanupJobAsync(Guid jobId, CancellationToken cancellationToken = default)
    {
        try
        {
            logger.LogDebug("Cleaning up job {JobId}", jobId);

            // Unsubscribe from job-specific channels
            await channelSubscriber.UnsubscribeFromJobChannelsAsync(jobId);

            // Release ownership from registry
            await ownershipRegistry.ReleaseAsync(jobId, cancellationToken);

            logger.LogInformation("Cleaned up job {JobId}", jobId);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error cleaning up job {JobId}", jobId);
        }
    }
}
