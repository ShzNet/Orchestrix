namespace Orchestrix.Schedules;

/// <summary>
/// Defines retry policy for failed jobs.
/// </summary>
public class RetryPolicy
{
    public int MaxRetries { get; set; } = 3;
    public TimeSpan InitialDelay { get; set; } = TimeSpan.FromSeconds(10);
    public double BackoffMultiplier { get; set; } = 2.0;
    public TimeSpan MaxDelay { get; set; } = TimeSpan.FromMinutes(10);
}
