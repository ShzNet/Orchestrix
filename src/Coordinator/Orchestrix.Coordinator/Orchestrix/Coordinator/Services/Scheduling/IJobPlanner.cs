using Orchestrix.Persistence.Entities;

namespace Orchestrix.Coordinator.Services.Scheduling;

/// <summary>
/// Interface for creating jobs from schedules.
/// </summary>
public interface IJobPlanner
{
    /// <summary>
    /// Creates a job from a cron schedule.
    /// </summary>
    /// <param name="schedule">The schedule to create a job from.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created job entity.</returns>
    Task<JobEntity> PlanJobFromScheduleAsync(CronScheduleEntity schedule, CancellationToken cancellationToken = default);
}
