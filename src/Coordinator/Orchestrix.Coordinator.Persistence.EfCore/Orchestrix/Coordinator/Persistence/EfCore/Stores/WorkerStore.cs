using Microsoft.EntityFrameworkCore;
using Orchestrix.Coordinator.Persistence.Entities;

namespace Orchestrix.Coordinator.Persistence.EfCore.Stores;

/// <summary>
/// Implementation of worker persistence using Entity Framework Core.
/// </summary>
public class WorkerStore(CoordinatorDbContext context) : IWorkerStore
{
    /// <inheritdoc />
    public async Task UpdateHeartbeatAsync(WorkerEntity worker, CancellationToken cancellationToken = default)
    {
        var existing = await context.Workers.FindAsync(new object[] { worker.WorkerId }, cancellationToken);
        if (existing == null)
        {
            context.Workers.Add(worker);
        }
        else
        {
            existing.LastHeartbeat = worker.LastHeartbeat;
            existing.Status = worker.Status;
            existing.CurrentLoad = worker.CurrentLoad;
            existing.Queues = worker.Queues;
            existing.Metadata = worker.Metadata;
            existing.MaxConcurrency = worker.MaxConcurrency;
        }
        await context.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task MarkDrainingAsync(string workerId, CancellationToken cancellationToken = default)
    {
        var worker = await context.Workers.FindAsync(new object[] { workerId }, cancellationToken);
        if (worker != null)
        {
            worker.Status = WorkerStatus.Draining;
            await context.SaveChangesAsync(cancellationToken);
        }
    }

    /// <inheritdoc />
    public async Task RemoveWorkerAsync(string workerId, CancellationToken cancellationToken = default)
    {
        var worker = await context.Workers.FindAsync(new object[] { workerId }, cancellationToken);
        if (worker != null)
        {
            context.Workers.Remove(worker);
            await context.SaveChangesAsync(cancellationToken);
        }
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<WorkerEntity>> GetAvailableWorkersForQueueAsync(string queue, CancellationToken cancellationToken = default)
    {
        // Simple filter in memory for array column if provider doesn't support array ops well, 
        // but Postgres does. Assuming usage of a provider that might not supports .Contains on arrays natively in all versions,
        // but Postgres usually does. 
        // Ideally we filter by queue. For now, fetch active and filter client side if complex.
        
        // Optimized:
        var workers = await context.Workers
            .Where(w => w.Status == WorkerStatus.Active && w.CurrentLoad < w.MaxConcurrency)
            .OrderBy(w => w.CurrentLoad)
            .ToListAsync(cancellationToken);

        return workers.Where(w => w.Queues.Contains(queue)).ToList();
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<WorkerEntity>> GetDeadWorkersAsync(TimeSpan timeout, CancellationToken cancellationToken = default)
    {
        var threshold = DateTimeOffset.UtcNow.Subtract(timeout);
        return await context.Workers
            .Where(w => w.Status != WorkerStatus.Offline && w.LastHeartbeat < threshold)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<WorkerEntity>> GetAllActiveWorkersAsync(CancellationToken cancellationToken = default)
    {
         return await context.Workers
            .Where(w => w.Status == WorkerStatus.Active)
            .ToListAsync(cancellationToken);
    }
}
