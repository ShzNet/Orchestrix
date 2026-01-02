using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Orchestrix.Coordinator.HostedServices.Base;
using Orchestrix.Coordinator.Services.Clustering;
using Orchestrix.Coordinator.Services.Scheduling;
using Orchestrix.Persistence;

namespace Orchestrix.Coordinator.HostedServices.Scheduling;

/// <summary>
/// Background service that scans due cron schedules and creates jobs.
/// Only runs on the Leader node.
/// </summary>
public class ScheduleScannerService : IntervalHostedService
{
    private readonly ILeaderElection _leaderElection;
    private readonly CoordinatorOptions _options;

    /// <summary>
    /// Initializes a new instance of <see cref="ScheduleScannerService"/>.
    /// </summary>
    /// <param name="serviceProvider">The service provider.</param>
    /// <param name="leaderElection">The leader election service.</param>
    /// <param name="options">The coordinator options.</param>
    /// <param name="logger">The logger.</param>
    public ScheduleScannerService(
        IServiceProvider serviceProvider,
        ILeaderElection leaderElection,
        CoordinatorOptions options,
        ILogger<ScheduleScannerService> logger)
        : base(serviceProvider, logger)
    {
        _leaderElection = leaderElection;
        _options = options;
    }

    /// <inheritdoc />
    protected override TimeSpan GetInterval() => _options.ScheduleScanInterval;

    /// <inheritdoc />
    protected override bool ShouldExecute() => _leaderElection.IsLeader;

    /// <inheritdoc />
    protected override async Task ExecuteScopedAsync(IServiceProvider scopedProvider, CancellationToken stoppingToken)
    {
        var scheduleStore = scopedProvider.GetRequiredService<ICronScheduleStore>();
        var jobPlanner = scopedProvider.GetRequiredService<IJobPlanner>();
        var logger = scopedProvider.GetRequiredService<ILogger<ScheduleScannerService>>();

        var now = DateTimeOffset.UtcNow;
        var dueSchedules = await scheduleStore.GetDueSchedulesAsync(now, stoppingToken);

        if (dueSchedules.Count == 0)
            return;

        logger.LogDebug("[ScheduleScanner] Found {Count} due schedules", dueSchedules.Count);

        foreach (var schedule in dueSchedules)
        {
            if (stoppingToken.IsCancellationRequested)
                break;

            try
            {
                await jobPlanner.PlanJobFromScheduleAsync(schedule, stoppingToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "[ScheduleScanner] Failed to create job from schedule {ScheduleId}", schedule.Id);
            }
        }
    }
}
