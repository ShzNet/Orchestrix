using Microsoft.EntityFrameworkCore;
using Orchestrix.Persistence.Entities;

namespace Orchestrix.Persistence.EfCore.Stores;

/// <summary>
/// Implementation of job history persistence using Entity Framework Core.
/// </summary>
public class JobHistoryStore(CoordinatorDbContext context) : IJobHistoryStore
{
    /// <inheritdoc />
    public async Task CreateAsync(JobHistoryEntity history, CancellationToken cancellationToken = default)
    {
        context.JobHistory.Add(history);
        await context.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<JobHistoryEntity>> GetByJobIdAsync(Guid jobId, CancellationToken cancellationToken = default)
    {
        return await context.JobHistory
            .Where(h => h.JobId == jobId)
            .OrderByDescending(h => h.StartedAt)
            .ToListAsync(cancellationToken);
    }
}
