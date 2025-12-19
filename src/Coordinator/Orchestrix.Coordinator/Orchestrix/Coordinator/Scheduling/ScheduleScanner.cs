using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Orchestrix.Coordinator.LeaderElection;
using Orchestrix.Coordinator.Persistence;
using Orchestrix.Coordinator.Persistence.Entities;

namespace Orchestrix.Coordinator.Scheduling;

/// <summary>
/// Background service that scans schedules and creates jobs (Leader only).
/// </summary>
internal class ScheduleScanner(
    ILeaderElection leaderElection,
    ICronScheduleStore cronScheduleStore,
    JobPlanner jobPlanner,
    IOptions<CoordinatorOptions> options,
    ILogger<ScheduleScanner> logger)
    : BackgroundService
{
    private readonly CoordinatorOptions _options = options.Value;
    private static readonly TimeSpan ScanInterval = TimeSpan.FromSeconds(10);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("ScheduleScanner started for node {NodeId}", _options.NodeId);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                // Only leader scans schedules
                if (leaderElection.IsLeader)
                {
                    await ScanCronSchedulesAsync(stoppingToken);
                }

                await Task.Delay(ScanInterval, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in schedule scanner loop");
                await Task.Delay(ScanInterval, stoppingToken);
            }
        }

        logger.LogInformation("ScheduleScanner stopped for node {NodeId}", _options.NodeId);
    }

    private async Task ScanCronSchedulesAsync(CancellationToken cancellationToken)
    {
        try
        {
            var now = DateTimeOffset.UtcNow;

            // Query due cron schedules
            var dueSchedules = await cronScheduleStore.GetDueSchedulesAsync(now, cancellationToken);

            foreach (var schedule in dueSchedules)
            {
                if (!ScheduleEvaluator.IsCronScheduleDue(schedule, now))
                    continue;

                // Plan and create job
                var job = await jobPlanner.PlanJobFromCronScheduleAsync(schedule, cancellationToken);

                if (job != null)
                {
                    logger.LogDebug(
                        "Created job {JobId} from cron schedule {ScheduleId}",
                        job.Id, schedule.Id);
                }
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error scanning cron schedules");
        }
    }
}
