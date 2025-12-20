using Microsoft.Extensions.Logging;
using Orchestrix.Coordinator.Caching;
using Orchestrix.Coordinator.Persistence;
using Orchestrix.Coordinator.Persistence.Entities;
using Orchestrix.Transport.Messages.Jobs;
using JobStatus = Orchestrix.JobStatus;

namespace Orchestrix.Coordinator.Ownership;

/// <summary>
/// Processes job events (status updates, logs) from workers.
/// </summary>
internal class JobEventProcessor(
    IJobStore jobStore,
    ILogStore logStore,
    ICacheInvalidator cacheInvalidator,
    JobOwnershipCleanup ownershipCleanup,
    ILogger<JobEventProcessor> logger)
{
    /// <summary>
    /// Processes job status update event.
    /// </summary>
    public async Task ProcessStatusUpdateAsync(JobStatusMessage message, CancellationToken cancellationToken = default)
    {
        try
        {
            logger.LogDebug(
                "Processing status update: JobId={JobId}, ExecutionId={ExecutionId}, Status={Status}",
                message.JobId, message.ExecutionId, message.Status);

            // Update job status in database
            // Cast from Orchestrix.Enums.JobStatus to Orchestrix.JobStatus
            var status = (JobStatus)(int)message.Status;
            await jobStore.UpdateStatusAsync(
                message.JobId,
                status,
                message.Error,
                message.CompletedAt,
                cancellationToken);

            // Invalidate cache after status update
            await cacheInvalidator.InvalidateJobAsync(message.JobId, cancellationToken);

            logger.LogInformation(
                "Updated job {JobId} status to {Status}",
                message.JobId, message.Status);

            // Cleanup if job reached terminal status
            if (IsTerminalStatus(status))
            {
                logger.LogDebug("Job {JobId} reached terminal status, triggering cleanup", message.JobId);
                await ownershipCleanup.CleanupJobAsync(message.JobId, cancellationToken);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                "Error processing status update for JobId={JobId}",
                message.JobId);
        }
    }

    /// <summary>
    /// Processes job log entry event.
    /// </summary>
    public async Task ProcessLogEntryAsync(JobLogMessage message, CancellationToken cancellationToken = default)
    {
        try
        {
            logger.LogDebug(
                "Processing log entry: JobId={JobId}, Level={Level}",
                message.JobId, message.Level);

            // Create log entry
            var logEntry = new LogEntry
            {
                JobId = message.JobId,
                Timestamp = DateTimeOffset.UtcNow,
                Level = message.Level.ToString(),
                Message = message.Message
            };

            // Append to log store
            await logStore.AppendAsync(logEntry, cancellationToken);

            // Note: Don't invalidate cache for logs (too frequent)
            // Logs are typically streamed, not cached
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                "Error processing log entry for JobId={JobId}",
                message.JobId);
        }
    }

    private static bool IsTerminalStatus(JobStatus status)
    {
        return status == JobStatus.Completed
            || status == JobStatus.Failed
            || status == JobStatus.Cancelled;
    }
}
