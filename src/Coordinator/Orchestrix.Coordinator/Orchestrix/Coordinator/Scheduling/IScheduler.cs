namespace Orchestrix.Coordinator.Scheduling;

/// <summary>
/// Service for scanning and evaluating schedules to create jobs.
/// </summary>
public interface IScheduler
{
    /// <summary>
    /// Scans cron schedules and creates jobs for due schedules.
    /// </summary>
    Task ScanCronSchedulesAsync(CancellationToken cancellationToken = default);
}
