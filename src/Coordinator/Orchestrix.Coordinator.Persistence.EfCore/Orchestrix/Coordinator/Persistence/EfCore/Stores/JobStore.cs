using Microsoft.EntityFrameworkCore;
using Orchestrix.Coordinator.Persistence.Entities;

namespace Orchestrix.Coordinator.Persistence.EfCore.Stores;

/// <summary>
/// Implementation of job persistence using Entity Framework Core.
/// </summary>
public class JobStore(CoordinatorDbContext context) : IJobStore
{
    /// <inheritdoc />
    public async Task EnqueueAsync(JobEntity job, CancellationToken cancellationToken = default)
    {
        context.Jobs.Add(job);
        await context.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task MarkDispatchedAsync(Guid jobId, string workerId, DateTimeOffset dispatchedAt, CancellationToken cancellationToken = default)
    {
        var job = await context.Jobs.FindAsync(new object[] { jobId }, cancellationToken);
        if (job != null)
        {
            job.WorkerId = workerId;
            job.DispatchedAt = dispatchedAt;
            job.Status = JobStatus.Running; // Assuming dispatched implies running state or pre-running
            await context.SaveChangesAsync(cancellationToken);
        }
    }

    /// <inheritdoc />
    public async Task UpdateStatusAsync(Guid jobId, JobStatus status, string? error = null, DateTimeOffset? completedAt = null, CancellationToken cancellationToken = default)
    {
        var job = await context.Jobs.FindAsync(new object[] { jobId }, cancellationToken);
        if (job != null)
        {
            job.Status = status;
            if (error != null) job.Error = error;
            if (completedAt != null) job.CompletedAt = completedAt;
            await context.SaveChangesAsync(cancellationToken);
        }
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<JobEntity>> GetPendingJobsAsync(int limit, CancellationToken cancellationToken = default)
    {
        return await context.Jobs
            .Where(j => j.Status == JobStatus.Pending && (j.ScheduledAt == null || j.ScheduledAt <= DateTimeOffset.UtcNow))
            .OrderByDescending(j => j.Priority)
            .ThenBy(j => j.CreatedAt)
            .Take(limit)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<JobEntity>> GetDueScheduledJobsAsync(DateTimeOffset now, int limit, CancellationToken cancellationToken = default)
    {
        return await context.Jobs
            .Where(j => j.Status == JobStatus.Pending && j.ScheduledAt != null && j.ScheduledAt <= now)
            .OrderBy(j => j.ScheduledAt)
            .Take(limit)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> TryClaimJobAsync(Guid jobId, string followerNodeId, CancellationToken cancellationToken = default)
    {
        try
        {
            var job = await context.Jobs.FindAsync([jobId], cancellationToken);
            if (job == null) return false;

            // In-memory check: if already claimed, fail immediately
            if (job.FollowerNodeId != null) return false;

            job.FollowerNodeId = followerNodeId;
            // Version will be auto-incremented by Context
            
            await context.SaveChangesAsync(cancellationToken);
            return true;
        }
        catch (DbUpdateConcurrencyException)
        {
            // Someone else modified the job (claimed it or other update)
            return false;
        }
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<JobEntity>> GetJobsByFollowerAsync(string followerNodeId, CancellationToken cancellationToken = default)
    {
        return await context.Jobs
            .Where(j => j.FollowerNodeId == followerNodeId)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task ReleaseJobsFromDeadFollowerAsync(string followerNodeId, CancellationToken cancellationToken = default)
    {
        await context.Jobs
            .Where(j => j.FollowerNodeId == followerNodeId)
            .ExecuteUpdateAsync(s => s.SetProperty(j => j.FollowerNodeId, (string?)null), cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> TryRetryJobAsync(Guid jobId, CancellationToken cancellationToken = default)
    {
        var job = await context.Jobs.FindAsync(new object[] { jobId }, cancellationToken);
        if (job == null || job.RetryCount >= job.MaxRetries) return false;

        job.RetryCount++;
        job.Status = JobStatus.Pending; // Reset to pending to be picked up again
        job.WorkerId = null;
        job.StartedAt = null;
        // Backoff logic could go here (update ScheduledAt)
        
        await context.SaveChangesAsync(cancellationToken);
        return true;
    }

    /// <inheritdoc />
    public async Task MoveToDeadLetterAsync(Guid jobId, CancellationToken cancellationToken = default)
    {
        var job = await context.Jobs.FindAsync(new object[] { jobId }, cancellationToken);
        if (job != null)
        {
            var deadLetter = new DeadLetterEntity
            {
                Id = Guid.NewGuid(),
                OriginalJobId = job.Id,
                JobType = job.JobType,
                ArgumentsJson = job.ArgumentsJson,
                LastError = job.Error ?? string.Empty,
                FailedAt = DateTimeOffset.UtcNow
            };
            context.DeadLetters.Add(deadLetter);
            context.Jobs.Remove(job); // Remove from active jobs? Or keep marked as failed? Usually move.
            await context.SaveChangesAsync(cancellationToken);
        }
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<JobEntity>> GetJobsNeedingCleanupAsync(int limit, CancellationToken cancellationToken = default)
    {
         return await context.Jobs
            .Where(j => (j.Status == JobStatus.Completed || j.Status == JobStatus.Failed || j.Status == JobStatus.Cancelled) 
                        && !j.ChannelsCleaned)
            .Take(limit)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task MarkChannelsCleanedAsync(Guid jobId, CancellationToken cancellationToken = default)
    {
        var job = await context.Jobs.FindAsync(new object[] { jobId }, cancellationToken);
        if (job != null)
        {
            job.ChannelsCleaned = true;
            await context.SaveChangesAsync(cancellationToken);
        }
    }

    /// <inheritdoc />
    public async Task<JobEntity?> GetByIdAsync(Guid jobId, CancellationToken cancellationToken = default)
    {
        return await context.Jobs.FindAsync(new object[] { jobId }, cancellationToken);
    }
}
