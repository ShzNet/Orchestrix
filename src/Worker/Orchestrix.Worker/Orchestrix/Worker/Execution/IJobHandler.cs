using Orchestrix.Jobs;

namespace Orchestrix.Worker.Execution;

/// <summary>
/// Interface for job handlers that process a specific job type.
/// </summary>
/// <typeparam name="TArgs">The type of the job arguments.</typeparam>
public interface IJobHandler<in TArgs>
{
    /// <summary>
    /// Handles the job execution.
    /// </summary>
    /// <param name="args">The job arguments.</param>
    /// <param name="context">The job execution context.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task HandleAsync(TArgs args, IJobContext context);
}

/// <summary>
/// Interface for job handlers that process jobs without typed arguments.
/// </summary>
public interface IJobHandler
{
    /// <summary>
    /// Handles the job execution.
    /// </summary>
    /// <param name="context">The job execution context.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task HandleAsync(IJobContext context);
}
