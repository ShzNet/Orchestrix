using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Orchestrix.Jobs;
using Orchestrix.Worker.Execution;

namespace Orchestrix.Worker;

/// <summary>
/// Interface for configuring worker job handlers.
/// </summary>
public interface IWorkerBuilder
{
    /// <summary>
    /// Registers a job handler for a specific job type.
    /// </summary>
    /// <typeparam name="THandler">The handler type.</typeparam>
    /// <typeparam name="TArgs">The arguments type.</typeparam>
    /// <param name="jobType">The job type name.</param>
    /// <returns>This builder for chaining.</returns>
    IWorkerBuilder AddJobHandler<THandler, TArgs>(string jobType)
        where THandler : class, IJobHandler<TArgs>;

    /// <summary>
    /// Registers a job handler for a specific job type (without typed arguments).
    /// </summary>
    /// <typeparam name="THandler">The handler type.</typeparam>
    /// <param name="jobType">The job type name.</param>
    /// <returns>This builder for chaining.</returns>
    IWorkerBuilder AddJobHandler<THandler>(string jobType)
        where THandler : class, IJobHandler;

    /// <summary>
    /// Scans the specified assembly for job handlers and registers them automatically.
    /// </summary>
    /// <param name="assembly">The assembly to scan.</param>
    /// <returns>This builder for chaining.</returns>
    IWorkerBuilder ScanAssembly(Assembly assembly);

    /// <summary>
    /// Scans the calling assembly for job handlers.
    /// </summary>
    /// <returns>This builder for chaining.</returns>
    IWorkerBuilder ScanAssembly();

    /// <summary>
    /// Adds a middleware to the job execution pipeline.
    /// </summary>
    /// <typeparam name="TMiddleware">The middleware type.</typeparam>
    /// <returns>This builder for chaining.</returns>
    IWorkerBuilder UseMiddleware<TMiddleware>()
        where TMiddleware : class, IJobMiddleware;

    /// <summary>
    /// Adds an inline middleware to the job execution pipeline.
    /// </summary>
    /// <param name="middleware">The middleware function.</param>
    /// <returns>This builder for chaining.</returns>
    IWorkerBuilder Use(Func<IJobContext, JobMiddlewareDelegate, Task> middleware);
}
