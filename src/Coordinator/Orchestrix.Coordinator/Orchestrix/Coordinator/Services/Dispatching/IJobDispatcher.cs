using Orchestrix.Persistence.Entities;

namespace Orchestrix.Coordinator.Services.Dispatching;

/// <summary>
/// Interface for dispatching jobs to workers.
/// </summary>
public interface IJobDispatcher
{
    /// <summary>
    /// Dispatches a job to workers via the transport layer.
    /// </summary>
    /// <param name="job">The job to dispatch.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The execution ID assigned to this dispatch.</returns>
    Task<Guid> DispatchAsync(JobEntity job, CancellationToken cancellationToken = default);
}
