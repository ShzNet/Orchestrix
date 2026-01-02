using Microsoft.Extensions.Logging;
using Orchestrix.Persistence;
using Orchestrix.Persistence.Entities;

namespace Orchestrix.Coordinator.Services.Scheduling;

/// <summary>
/// Implementation of job planning from schedules.
/// </summary>
public class JobPlanner : IJobPlanner
{
    private readonly IJobStore _jobStore;
    private readonly ICronScheduleStore _scheduleStore;
    private readonly IScheduleEvaluator _scheduleEvaluator;
    private readonly ILogger<JobPlanner> _logger;

    /// <summary>
    /// Initializes a new instance of <see cref="JobPlanner"/>.
    /// </summary>
    /// <param name="jobStore">The job store.</param>
    /// <param name="scheduleStore">The schedule store.</param>
    /// <param name="scheduleEvaluator">The schedule evaluator.</param>
    /// <param name="logger">The logger.</param>
    public JobPlanner(
        IJobStore jobStore,
        ICronScheduleStore scheduleStore,
        IScheduleEvaluator scheduleEvaluator,
        ILogger<JobPlanner> logger)
    {
        _jobStore = jobStore;
        _scheduleStore = scheduleStore;
        _scheduleEvaluator = scheduleEvaluator;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<JobEntity> PlanJobFromScheduleAsync(CronScheduleEntity schedule, CancellationToken cancellationToken = default)
    {
        var now = DateTimeOffset.UtcNow;

        // Create job from schedule
        var job = new JobEntity
        {
            Id = Guid.NewGuid(),
            JobType = schedule.JobType,
            Queue = schedule.Queue,
            ArgumentsJson = schedule.ArgumentsJson ?? "{}",
            Status = JobStatus.Pending,
            Priority = 0, // Scheduled jobs have default priority
            ScheduleId = schedule.Id,
            ScheduleType = "Cron",
            CreatedAt = now,
            ScheduledAt = schedule.NextRunTime // Schedule for original planned time
        };

        // Enqueue the job
        await _jobStore.EnqueueAsync(job, cancellationToken);

        _logger.LogInformation(
            "[JobPlanner] Created job {JobId} from schedule {ScheduleName} ({ScheduleId}), JobType: {JobType}",
            job.Id, schedule.Name, schedule.Id, schedule.JobType);

        // Calculate and update next run time
        var nextRunTime = _scheduleEvaluator.CalculateNextRunTime(schedule, now);
        if (nextRunTime.HasValue)
        {
            await _scheduleStore.UpdateNextRunTimeAsync(
                schedule.Id,
                nextRunTime.Value,
                now,
                cancellationToken);

            _logger.LogDebug(
                "[JobPlanner] Updated schedule {ScheduleId} next run time to {NextRunTime}",
                schedule.Id, nextRunTime.Value);
        }
        else
        {
            _logger.LogWarning(
                "[JobPlanner] Could not calculate next run time for schedule {ScheduleId}",
                schedule.Id);
        }

        return job;
    }
}
