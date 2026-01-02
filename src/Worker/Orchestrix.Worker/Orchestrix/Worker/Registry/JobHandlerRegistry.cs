using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

namespace Orchestrix.Worker.Registry;

/// <summary>
/// Implementation of job handler registry.
/// </summary>
public class JobHandlerRegistry : IJobHandlerRegistry
{
    private readonly ConcurrentDictionary<string, (Type HandlerType, Type? ArgsType)> _handlers = new();
    private readonly ILogger<JobHandlerRegistry> _logger;

    /// <summary>
    /// Initializes a new instance of <see cref="JobHandlerRegistry"/>.
    /// </summary>
    /// <param name="logger">The logger.</param>
    public JobHandlerRegistry(ILogger<JobHandlerRegistry> logger)
    {
        _logger = logger;
    }

    /// <inheritdoc />
    public Type? GetHandlerType(string jobType)
    {
        if (_handlers.TryGetValue(jobType, out var handlerInfo))
        {
            return handlerInfo.HandlerType;
        }
        return null;
    }

    /// <inheritdoc />
    public Type? GetArgumentsType(string jobType)
    {
        if (_handlers.TryGetValue(jobType, out var handlerInfo))
        {
            return handlerInfo.ArgsType;
        }
        return null;
    }

    /// <inheritdoc />
    public void Register<THandler, TArgs>(string jobType)
        where THandler : class
    {
        if (_handlers.TryAdd(jobType, (typeof(THandler), typeof(TArgs))))
        {
            _logger.LogInformation(
                "[JobHandlerRegistry] Registered handler {HandlerType} for job type '{JobType}' with args {ArgsType}",
                typeof(THandler).Name, jobType, typeof(TArgs).Name);
        }
        else
        {
            _logger.LogWarning(
                "[JobHandlerRegistry] Handler for job type '{JobType}' already registered",
                jobType);
        }
    }

    /// <inheritdoc />
    public void Register<THandler>(string jobType)
        where THandler : class
    {
        if (_handlers.TryAdd(jobType, (typeof(THandler), null)))
        {
            _logger.LogInformation(
                "[JobHandlerRegistry] Registered handler {HandlerType} for job type '{JobType}' (no args)",
                typeof(THandler).Name, jobType);
        }
        else
        {
            _logger.LogWarning(
                "[JobHandlerRegistry] Handler for job type '{JobType}' already registered",
                jobType);
        }
    }

    /// <summary>
    /// Registers a handler using runtime types (non-generic).
    /// </summary>
    /// <param name="jobType">The job type name.</param>
    /// <param name="handlerType">The handler type.</param>
    /// <param name="argsType">The arguments type, or null.</param>
    public void RegisterHandler(string jobType, Type handlerType, Type? argsType)
    {
        if (_handlers.TryAdd(jobType, (handlerType, argsType)))
        {
            _logger.LogInformation(
                "[JobHandlerRegistry] Registered handler {HandlerType} for job type '{JobType}'",
                handlerType.Name, jobType);
        }
        else
        {
            _logger.LogWarning(
                "[JobHandlerRegistry] Handler for job type '{JobType}' already registered",
                jobType);
        }
    }
}
