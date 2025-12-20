using System.Collections.Concurrent;

namespace Orchestrix.Coordinator.Ownership;

/// <summary>
/// In-memory registry for tracking job ownership.
/// </summary>
internal class JobOwnershipRegistry : IJobOwnershipRegistry
{
    private readonly ConcurrentDictionary<Guid, JobOwnershipInfo> _ownedJobs = new();

    public Task<bool> ClaimAsync(
        Guid jobId,
        string nodeId,
        Guid executionId,
        CancellationToken cancellationToken = default)
    {
        var info = new JobOwnershipInfo
        {
            JobId = jobId,
            NodeId = nodeId,
            ExecutionId = executionId,
            ClaimedAt = DateTimeOffset.UtcNow
        };

        var claimed = _ownedJobs.TryAdd(jobId, info);
        return Task.FromResult(claimed);
    }

    public Task ReleaseAsync(Guid jobId, CancellationToken cancellationToken = default)
    {
        _ownedJobs.TryRemove(jobId, out _);
        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<JobOwnershipInfo>> GetOwnedJobsAsync(
        CancellationToken cancellationToken = default)
    {
        var jobs = _ownedJobs.Values.ToList();
        return Task.FromResult<IReadOnlyList<JobOwnershipInfo>>(jobs);
    }

    public Task<bool> OwnsJobAsync(Guid jobId, CancellationToken cancellationToken = default)
    {
        var owns = _ownedJobs.ContainsKey(jobId);
        return Task.FromResult(owns);
    }

    public Task<JobOwnershipInfo?> GetOwnershipInfoAsync(
        Guid jobId,
        CancellationToken cancellationToken = default)
    {
        _ownedJobs.TryGetValue(jobId, out var info);
        return Task.FromResult<JobOwnershipInfo?>(info);
    }
}
