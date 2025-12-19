using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Orchestrix.Coordinator.Persistence;
using Orchestrix.Coordinator.Persistence.Entities;

namespace Orchestrix.Coordinator.Scheduling;

/// <summary>
/// Plans and creates jobs from schedules.
/// </summary>
internal class JobPlanner(
    IJobStore jobStore,
    ICronScheduleStore cronScheduleStore,
    IOptions<CoordinatorOptions> options,
    ILogger<JobPlanner> logger)
{
    private readonly CoordinatorOptions _options = options.Value;

    /// <summary>
    /// Plans a job from a cron schedule.
    /// </summary>
    public async Task<JobEntity?> PlanJobFromCronScheduleAsync(
        CronScheduleEntity schedule,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Create job entity
            var job = new JobEntity
            {
                Id = Guid.NewGuid(),
                Queue = schedule.Queue,
                JobType = schedule.JobType,
                ArgumentsJson = schedule.ArgumentsJson ?? string.Empty,
                Status = JobStatus.Pending,
                CreatedAt = DateTimeOffset.UtcNow,
                ScheduleId = schedule.Id,
                ScheduleType = "Cron"
            };

            // Save job
            await jobStore.EnqueueAsync(job, cancellationToken);

            // Update schedule's next run time
            var nextRunTime = ScheduleEvaluator.CalculateNextCronRunTime(
                schedule.CronExpression,
                DateTimeOffset.UtcNow,
                schedule.TimeZone);

            if (nextRunTime.HasValue)
            {
                await cronScheduleStore.UpdateNextRunTimeAsync(
                    schedule.Id,
                    nextRunTime.Value,
                    DateTimeOffset.UtcNow,
                    cancellationToken);
            }

            logger.LogInformation(
                "Planned job {JobId} from cron schedule {ScheduleId} ({CronExpression})",
                job.Id, schedule.Id, schedule.CronExpression);

            return job;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error planning job from cron schedule {ScheduleId}", schedule.Id);
            return null;
        }
    }
}
