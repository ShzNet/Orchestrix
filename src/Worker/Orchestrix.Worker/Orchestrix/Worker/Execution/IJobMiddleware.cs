using Orchestrix.Jobs;

namespace Orchestrix.Worker.Execution;

/// <summary>
/// Delegate representing the next middleware in the pipeline.
/// </summary>
/// <param name="context">The job context.</param>
/// <returns>A task representing the asynchronous operation.</returns>
public delegate Task JobMiddlewareDelegate(IJobContext context);

/// <summary>
/// Interface for job execution middleware.
/// Allows running code before and after job handlers.
/// </summary>
public interface IJobMiddleware
{
    /// <summary>
    /// Invokes the middleware.
    /// </summary>
    /// <param name="context">The job context.</param>
    /// <param name="next">The next middleware in the pipeline.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task InvokeAsync(IJobContext context, JobMiddlewareDelegate next);
}
