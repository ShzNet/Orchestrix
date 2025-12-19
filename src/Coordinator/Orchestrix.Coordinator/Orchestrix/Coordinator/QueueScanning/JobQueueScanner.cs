using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Orchestrix.Coordinator.Dispatching;
using Orchestrix.Coordinator.LeaderElection;
using Orchestrix.Coordinator.Persistence;

namespace Orchestrix.Coordinator.QueueScanning;

/// <summary>
/// Background service that scans pending jobs and dispatches them to workers (Leader only).
/// </summary>
internal class JobQueueScanner(
    ILeaderElection leaderElection,
    IJobStore jobStore,
    IJobDispatcher jobDispatcher,
    IOptions<CoordinatorOptions> options,
    ILogger<JobQueueScanner> logger)
    : BackgroundService
{
    private readonly CoordinatorOptions _options = options.Value;
    private static readonly TimeSpan ScanInterval = TimeSpan.FromSeconds(5);
    private const int BatchSize = 100;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("JobQueueScanner started for node {NodeId}", _options.NodeId);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                // Only leader scans and dispatches jobs
                if (leaderElection.IsLeader)
                {
                    await ScanAndDispatchJobsAsync(stoppingToken);
                }

                await Task.Delay(ScanInterval, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in job queue scanner loop");
                await Task.Delay(ScanInterval, stoppingToken);
            }
        }

        logger.LogInformation("JobQueueScanner stopped for node {NodeId}", _options.NodeId);
    }

    private async Task ScanAndDispatchJobsAsync(CancellationToken cancellationToken)
    {
        try
        {
            // Query pending jobs ready for dispatch
            var pendingJobs = await jobStore.GetPendingJobsAsync(BatchSize, cancellationToken);

            if (pendingJobs.Count == 0)
                return;

            logger.LogDebug(
                "Found {Count} pending jobs to dispatch",
                pendingJobs.Count);

            // Dispatch each job
            foreach (var job in pendingJobs)
            {
                try
                {
                    await jobDispatcher.DispatchAsync(job, cancellationToken);

                    logger.LogDebug(
                        "Dispatched job {JobId} to queue {Queue}",
                        job.Id, job.Queue);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex,
                        "Error dispatching job {JobId}",
                        job.Id);
                    // Continue with next job
                }
            }

            logger.LogInformation(
                "Dispatched {Count} jobs in this scan",
                pendingJobs.Count);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error scanning pending jobs");
        }
    }
}
