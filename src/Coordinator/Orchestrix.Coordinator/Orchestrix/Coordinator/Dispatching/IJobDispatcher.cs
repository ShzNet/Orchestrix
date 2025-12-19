using Orchestrix.Coordinator.Persistence.Entities;

namespace Orchestrix.Coordinator.Dispatching;

/// <summary>
/// Service for dispatching jobs to workers via transport.
/// </summary>
public interface IJobDispatcher
{
    /// <summary>
    /// Dispatches a job to workers for execution.
    /// </summary>
    /// <param name="job">The job to dispatch.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task DispatchAsync(JobEntity job, CancellationToken cancellationToken = default);
}
