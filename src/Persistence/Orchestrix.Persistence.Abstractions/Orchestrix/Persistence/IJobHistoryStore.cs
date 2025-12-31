using Orchestrix.Persistence.Entities;

namespace Orchestrix.Persistence;

/// <summary>
/// Repository interface for job history persistence operations.
/// </summary>
public interface IJobHistoryStore
{
    /// <summary>
    /// Creates a new job history record.
    /// </summary>
    Task CreateAsync(JobHistoryEntity history, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all history records for a specific job.
    /// </summary>
    Task<IReadOnlyList<JobHistoryEntity>> GetByJobIdAsync(Guid jobId, CancellationToken cancellationToken = default);
}
