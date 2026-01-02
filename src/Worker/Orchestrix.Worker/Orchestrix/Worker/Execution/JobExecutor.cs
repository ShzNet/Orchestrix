using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Orchestrix.Enums;
using Orchestrix.Jobs;
using Orchestrix.Transport;
using Orchestrix.Transport.Messages.Jobs;
using Orchestrix.Worker.Registry;

namespace Orchestrix.Worker.Execution;

/// <summary>
/// Implementation of job execution with middleware pipeline support.
/// </summary>
public class JobExecutor : IJobExecutor
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IJobHandlerRegistry _registry;
    private readonly IPublisher _publisher;
    private readonly TransportChannels _channels;
    private readonly WorkerOptions _options;
    private readonly IReadOnlyList<Type> _middlewareTypes;
    private readonly IReadOnlyList<Func<IJobContext, JobMiddlewareDelegate, Task>> _inlineMiddlewares;
    private readonly ILogger<JobExecutor> _logger;

    /// <summary>
    /// Initializes a new instance of <see cref="JobExecutor"/>.
    /// </summary>
    public JobExecutor(
        IServiceProvider serviceProvider,
        IJobHandlerRegistry registry,
        IPublisher publisher,
        TransportChannels channels,
        WorkerOptions options,
        IReadOnlyList<Type> middlewareTypes,
        IReadOnlyList<Func<IJobContext, JobMiddlewareDelegate, Task>> inlineMiddlewares,
        ILogger<JobExecutor> logger)
    {
        _serviceProvider = serviceProvider;
        _registry = registry;
        _publisher = publisher;
        _channels = channels;
        _options = options;
        _middlewareTypes = middlewareTypes;
        _inlineMiddlewares = inlineMiddlewares;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<bool> ExecuteAsync(JobDispatchMessage message, CancellationToken cancellationToken = default)
    {
        var handlerType = _registry.GetHandlerType(message.JobType);
        if (handlerType == null)
        {
            _logger.LogError("[JobExecutor] No handler registered for job type '{JobType}'", message.JobType);
            await PublishStatusAsync(message, JobStatus.Failed, "No handler registered for job type");
            return false;
        }

        // Create linked cancellation token with timeout
        using var timeoutCts = new CancellationTokenSource(_options.DefaultJobTimeout);
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutCts.Token);

        // Create job context
        var context = new JobContext(
            message,
            "default", // Queue is not in message, would need to track
            linkedCts.Token,
            _publisher,
            _channels,
            _logger);

        try
        {
            // Publish Running status
            await PublishStatusAsync(message, JobStatus.Running, null);

            _logger.LogInformation(
                "[JobExecutor] Starting job {JobId} (ExecutionId: {ExecutionId}, Type: {JobType})",
                message.JobId, message.ExecutionId, message.JobType);

            // Resolve handler from DI
            using var scope = _serviceProvider.CreateScope();
            
            // Build middleware pipeline
            var pipeline = BuildPipeline(scope.ServiceProvider, handlerType, message.JobType);
            
            // Execute pipeline
            await pipeline(context);

            _logger.LogInformation(
                "[JobExecutor] Completed job {JobId} (ExecutionId: {ExecutionId})",
                message.JobId, message.ExecutionId);

            await PublishStatusAsync(message, JobStatus.Completed, null);
            return true;
        }
        catch (OperationCanceledException) when (timeoutCts.IsCancellationRequested)
        {
            _logger.LogWarning(
                "[JobExecutor] Job {JobId} timed out after {Timeout}",
                message.JobId, _options.DefaultJobTimeout);

            await PublishStatusAsync(message, JobStatus.Failed, $"Job timed out after {_options.DefaultJobTimeout}");
            return false;
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("[JobExecutor] Job {JobId} was cancelled", message.JobId);
            await PublishStatusAsync(message, JobStatus.Cancelled, "Job was cancelled");
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[JobExecutor] Job {JobId} failed with error", message.JobId);
            await PublishStatusAsync(message, JobStatus.Failed, ex.Message);
            return false;
        }
    }

    private JobMiddlewareDelegate BuildPipeline(IServiceProvider scopedProvider, Type handlerType, string jobType)
    {
        // Start with the innermost handler execution
        JobMiddlewareDelegate handler = async ctx =>
        {
            await ExecuteHandlerAsync(scopedProvider, handlerType, jobType, ctx);
        };

        // Wrap with inline middlewares (in reverse order)
        for (var i = _inlineMiddlewares.Count - 1; i >= 0; i--)
        {
            var middleware = _inlineMiddlewares[i];
            var next = handler;
            handler = ctx => middleware(ctx, next);
        }

        // Wrap with typed middlewares (in reverse order)
        for (var i = _middlewareTypes.Count - 1; i >= 0; i--)
        {
            var middlewareType = _middlewareTypes[i];
            var middlewareInstance = scopedProvider.GetService(middlewareType) as IJobMiddleware;
            if (middlewareInstance != null)
            {
                var next = handler;
                handler = ctx => middlewareInstance.InvokeAsync(ctx, next);
            }
        }

        return handler;
    }

    private async Task ExecuteHandlerAsync(IServiceProvider scopedProvider, Type handlerType, string jobType, IJobContext context)
    {
        var handler = scopedProvider.GetService(handlerType);
        if (handler == null)
        {
            throw new InvalidOperationException($"Failed to resolve handler {handlerType.Name}");
        }

        var argsType = _registry.GetArgumentsType(jobType);

        if (argsType != null)
        {
            // Get message arguments from context
            var jobContext = (JobContext)context;
            var message = jobContext.Message;
            
            var args = JsonSerializer.Deserialize(message.Arguments, argsType);
            if (args == null)
            {
                throw new InvalidOperationException("Failed to deserialize job arguments");
            }

            var handleMethod = handlerType.GetMethod("HandleAsync");
            if (handleMethod != null)
            {
                var task = handleMethod.Invoke(handler, [args, context]) as Task;
                if (task != null)
                {
                    await task;
                }
            }
        }
        else
        {
            if (handler is IJobHandler simpleHandler)
            {
                await simpleHandler.HandleAsync(context);
            }
        }
    }

    private async Task PublishStatusAsync(JobDispatchMessage message, JobStatus status, string? errorMessage)
    {
        var statusMessage = new JobStatusMessage
        {
            JobId = message.JobId,
            ExecutionId = message.ExecutionId,
            Status = status,
            Error = errorMessage,
            CompletedAt = status == JobStatus.Completed || status == JobStatus.Failed || status == JobStatus.Cancelled
                ? DateTimeOffset.UtcNow
                : null
        };

        var channel = _channels.JobStatus(message.ExecutionId);
        await _publisher.PublishAsync(channel, statusMessage);
    }
}
