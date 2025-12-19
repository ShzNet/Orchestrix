using Microsoft.Extensions.Logging;
using Orchestrix.Coordinator.Persistence;
using Orchestrix.Coordinator.Persistence.Entities;

namespace Orchestrix.Coordinator.Scheduling;

/// <summary>
/// Implementation of IScheduler.
/// </summary>
internal class Scheduler(
    ICronScheduleStore cronScheduleStore,
    JobPlanner jobPlanner)
    : IScheduler
{
    public async Task ScanCronSchedulesAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTimeOffset.UtcNow;
        var dueSchedules = await cronScheduleStore.GetDueSchedulesAsync(now, cancellationToken);

        foreach (var schedule in dueSchedules)
        {
            if (ScheduleEvaluator.IsCronScheduleDue(schedule, now))
            {
                await jobPlanner.PlanJobFromCronScheduleAsync(schedule, cancellationToken);
            }
        }
    }
}
