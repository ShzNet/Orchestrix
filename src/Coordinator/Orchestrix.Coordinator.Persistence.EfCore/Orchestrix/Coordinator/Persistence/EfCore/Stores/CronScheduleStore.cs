using Microsoft.EntityFrameworkCore;
using Orchestrix.Coordinator.Persistence.Entities;

namespace Orchestrix.Coordinator.Persistence.EfCore.Stores;

/// <summary>
/// Implementation of cron schedule persistence using Entity Framework Core.
/// </summary>
public class CronScheduleStore(CoordinatorDbContext context) : ICronScheduleStore
{
    /// <inheritdoc />
    public async Task RegisterScheduleAsync(CronScheduleEntity schedule, CancellationToken cancellationToken = default)
    {
        context.CronSchedules.Add(schedule);
        await context.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task UpdateNextRunTimeAsync(Guid scheduleId, DateTimeOffset nextRunTime, DateTimeOffset lastRunTime, CancellationToken cancellationToken = default)
    {
        var schedule = await context.CronSchedules.FindAsync(new object[] { scheduleId }, cancellationToken);
        if (schedule != null)
        {
            schedule.NextRunTime = nextRunTime;
            schedule.LastRunTime = lastRunTime;
            await context.SaveChangesAsync(cancellationToken);
        }
    }

    /// <inheritdoc />
    public async Task SetEnabledAsync(Guid scheduleId, bool enabled, CancellationToken cancellationToken = default)
    {
        var schedule = await context.CronSchedules.FindAsync(new object[] { scheduleId }, cancellationToken);
        if (schedule != null)
        {
            schedule.IsEnabled = enabled;
            await context.SaveChangesAsync(cancellationToken);
        }
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<CronScheduleEntity>> GetDueSchedulesAsync(DateTimeOffset now, CancellationToken cancellationToken = default)
    {
        return await context.CronSchedules
            .Where(s => s.IsEnabled && s.NextRunTime != null && s.NextRunTime <= now)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task RemoveScheduleAsync(Guid scheduleId, CancellationToken cancellationToken = default)
    {
        var schedule = await context.CronSchedules.FindAsync(new object[] { scheduleId }, cancellationToken);
        if (schedule != null)
        {
            context.CronSchedules.Remove(schedule);
            await context.SaveChangesAsync(cancellationToken);
        }
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<CronScheduleEntity>> GetAllSchedulesAsync(CancellationToken cancellationToken = default)
    {
        return await context.CronSchedules.ToListAsync(cancellationToken);
    }
}
