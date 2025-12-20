using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Orchestrix.Coordinator.Caching;

namespace Orchestrix.Coordinator.Services;

/// <summary>
/// Implementation of cache invalidation service.
/// </summary>
internal class CacheInvalidator : ICacheInvalidator
{
    private readonly IDistributedCache _cache;
    private readonly string _prefix;

    public CacheInvalidator(
        IDistributedCache cache,
        IOptions<CoordinatorOptions> options)
    {
        _cache = cache;
        _prefix = options.Value.CachePrefix;
    }

    public Task InvalidateJobAsync(Guid jobId, CancellationToken cancellationToken = default)
    {
        return _cache.RemoveAsync(CacheKeys.Job(_prefix, jobId), cancellationToken);
    }

    public Task InvalidateJobsByQueueAsync(string queue, CancellationToken cancellationToken = default)
    {
        return _cache.RemoveAsync(CacheKeys.JobsByQueue(_prefix, queue), cancellationToken);
    }

    public Task InvalidateJobsByStatusAsync(string status, CancellationToken cancellationToken = default)
    {
        return _cache.RemoveAsync(CacheKeys.JobsByStatus(_prefix, status), cancellationToken);
    }

    public Task InvalidateScheduleAsync(Guid scheduleId, CancellationToken cancellationToken = default)
    {
        return _cache.RemoveAsync(CacheKeys.Schedule(_prefix, scheduleId), cancellationToken);
    }

    public Task InvalidateAllSchedulesAsync(CancellationToken cancellationToken = default)
    {
        return _cache.RemoveAsync(CacheKeys.AllSchedules(_prefix), cancellationToken);
    }

    public Task InvalidateWorkerAsync(string workerId, CancellationToken cancellationToken = default)
    {
        return _cache.RemoveAsync(CacheKeys.Worker(_prefix, workerId), cancellationToken);
    }

    public Task InvalidateAllWorkersAsync(CancellationToken cancellationToken = default)
    {
        return _cache.RemoveAsync(CacheKeys.AllWorkers(_prefix), cancellationToken);
    }

    public Task InvalidateCoordinatorNodeAsync(string nodeId, CancellationToken cancellationToken = default)
    {
        return _cache.RemoveAsync(CacheKeys.CoordinatorNode(_prefix, nodeId), cancellationToken);
    }

    public Task InvalidateAllCoordinatorNodesAsync(CancellationToken cancellationToken = default)
    {
        return _cache.RemoveAsync(CacheKeys.AllCoordinatorNodes(_prefix), cancellationToken);
    }
}
