using Orchestrix.Transport.Messages.Jobs;

namespace Orchestrix.Worker.Execution;

/// <summary>
/// Interface for executing jobs.
/// </summary>
public interface IJobExecutor
{
    /// <summary>
    /// Executes a job from a dispatch message.
    /// </summary>
    /// <param name="message">The job dispatch message.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if job completed successfully, false otherwise.</returns>
    Task<bool> ExecuteAsync(JobDispatchMessage message, CancellationToken cancellationToken = default);
}
