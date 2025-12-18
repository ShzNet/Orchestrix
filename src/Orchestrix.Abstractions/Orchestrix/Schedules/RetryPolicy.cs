namespace Orchestrix.Schedules;

/// <summary>
/// Represents the retry policy configuration for a job.
/// </summary>
public record RetryPolicy
{
    /// <summary>
    /// Gets the maximum number of retry attempts.
    /// </summary>
    public int MaxRetries { get; init; } = 3;

    /// <summary>
    /// Gets the initial delay before the first retry.
    /// </summary>
    public TimeSpan InitialDelay { get; init; } = TimeSpan.FromSeconds(10);

    /// <summary>
    /// Gets the multiplier for exponential backoff.
    /// </summary>
    public double BackoffMultiplier { get; init; } = 2.0;

    /// <summary>
    /// Gets the maximum delay between retries.
    /// </summary>
    public TimeSpan MaxDelay { get; init; } = TimeSpan.FromMinutes(10);
}
