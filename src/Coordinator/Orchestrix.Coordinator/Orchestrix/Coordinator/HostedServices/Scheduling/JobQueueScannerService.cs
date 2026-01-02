using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Orchestrix.Coordinator.HostedServices.Base;
using Orchestrix.Coordinator.Services.Clustering;
using Orchestrix.Coordinator.Services.Dispatching;
using Orchestrix.Persistence;

namespace Orchestrix.Coordinator.HostedServices.Scheduling;

/// <summary>
/// Background service that scans pending jobs and dispatches them to workers.
/// Only runs on the Leader node.
/// </summary>
public class JobQueueScannerService : IntervalHostedService
{
    private readonly ILeaderElection _leaderElection;
    private readonly CoordinatorOptions _options;

    /// <summary>
    /// Initializes a new instance of <see cref="JobQueueScannerService"/>.
    /// </summary>
    /// <param name="serviceProvider">The service provider.</param>
    /// <param name="leaderElection">The leader election service.</param>
    /// <param name="options">The coordinator options.</param>
    /// <param name="logger">The logger.</param>
    public JobQueueScannerService(
        IServiceProvider serviceProvider,
        ILeaderElection leaderElection,
        CoordinatorOptions options,
        ILogger<JobQueueScannerService> logger)
        : base(serviceProvider, logger)
    {
        _leaderElection = leaderElection;
        _options = options;
    }

    /// <inheritdoc />
    protected override TimeSpan GetInterval() => _options.JobQueueScanInterval;

    /// <inheritdoc />
    protected override bool ShouldExecute() => _leaderElection.IsLeader;

    /// <inheritdoc />
    protected override async Task ExecuteScopedAsync(IServiceProvider scopedProvider, CancellationToken stoppingToken)
    {
        var jobStore = scopedProvider.GetRequiredService<IJobStore>();
        var jobDispatcher = scopedProvider.GetRequiredService<IJobDispatcher>();
        var logger = scopedProvider.GetRequiredService<ILogger<JobQueueScannerService>>();

        // Get pending jobs ready for dispatch
        var pendingJobs = await jobStore.GetPendingJobsAsync(_options.JobQueueBatchSize, stoppingToken);

        if (pendingJobs.Count == 0)
            return;

        logger.LogDebug("[JobQueueScanner] Found {Count} pending jobs to dispatch", pendingJobs.Count);

        foreach (var job in pendingJobs)
        {
            if (stoppingToken.IsCancellationRequested)
                break;

            try
            {
                await jobDispatcher.DispatchAsync(job, stoppingToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "[JobQueueScanner] Failed to dispatch job {JobId}", job.Id);
            }
        }
    }
}
