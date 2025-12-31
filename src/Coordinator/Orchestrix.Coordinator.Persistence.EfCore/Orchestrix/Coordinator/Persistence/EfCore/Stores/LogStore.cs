using Microsoft.EntityFrameworkCore;
using Orchestrix.Coordinator.Persistence.Entities;

namespace Orchestrix.Coordinator.Persistence.EfCore.Stores;

/// <summary>
/// Implementation of log persistence using Entity Framework Core.
/// </summary>
public class LogStore(CoordinatorDbContext context) : ILogStore
{
    /// <inheritdoc />
    public async Task AppendAsync(LogEntry logEntry, CancellationToken cancellationToken = default)
    {
        context.Logs.Add(logEntry);
        await context.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<LogEntry>> GetByJobIdAsync(Guid jobId, CancellationToken cancellationToken = default)
    {
        return await context.Logs
            .Where(l => l.JobId == jobId)
            .OrderBy(l => l.Timestamp)
            .ToListAsync(cancellationToken);
    }
}
