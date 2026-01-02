using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Orchestrix.Jobs;
using Orchestrix.Worker.Execution;

namespace Orchestrix.Worker;

/// <summary>
/// Internal implementation of worker builder.
/// </summary>
internal class WorkerBuilder : IWorkerBuilder
{
    private readonly IServiceCollection _services;
    private readonly List<(string JobType, Type HandlerType, Type? ArgsType)> _handlers = new();
    private readonly List<Type> _middlewareTypes = new();
    private readonly List<Func<IJobContext, JobMiddlewareDelegate, Task>> _inlineMiddlewares = new();

    public WorkerBuilder(IServiceCollection services)
    {
        _services = services;
    }

    internal IReadOnlyList<(string JobType, Type HandlerType, Type? ArgsType)> Handlers => _handlers;
    internal IReadOnlyList<Type> MiddlewareTypes => _middlewareTypes;
    internal IReadOnlyList<Func<IJobContext, JobMiddlewareDelegate, Task>> InlineMiddlewares => _inlineMiddlewares;

    /// <inheritdoc />
    public IWorkerBuilder AddJobHandler<THandler, TArgs>(string jobType)
        where THandler : class, IJobHandler<TArgs>
    {
        _services.AddScoped<THandler>();
        _handlers.Add((jobType, typeof(THandler), typeof(TArgs)));
        return this;
    }

    /// <inheritdoc />
    public IWorkerBuilder AddJobHandler<THandler>(string jobType)
        where THandler : class, IJobHandler
    {
        _services.AddScoped<THandler>();
        _handlers.Add((jobType, typeof(THandler), null));
        return this;
    }

    /// <inheritdoc />
    public IWorkerBuilder ScanAssembly(Assembly assembly)
    {
        var handlerTypes = assembly.GetTypes()
            .Where(t => t is { IsClass: true, IsAbstract: false })
            .Where(IsJobHandler);

        foreach (var handlerType in handlerTypes)
        {
            var jobTypeAttr = handlerType.GetCustomAttribute<JobTypeAttribute>();
            var jobTypeName = jobTypeAttr?.JobType ?? handlerType.Name;

            // Find implemented IJobHandler<TArgs> interface
            var genericInterface = handlerType.GetInterfaces()
                .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IJobHandler<>));

            if (genericInterface != null)
            {
                var argsType = genericInterface.GetGenericArguments()[0];
                _services.AddScoped(handlerType);
                _handlers.Add((jobTypeName, handlerType, argsType));
            }
            else if (typeof(IJobHandler).IsAssignableFrom(handlerType))
            {
                _services.AddScoped(handlerType);
                _handlers.Add((jobTypeName, handlerType, null));
            }
        }

        return this;
    }

    /// <inheritdoc />
    public IWorkerBuilder ScanAssembly()
    {
        return ScanAssembly(Assembly.GetCallingAssembly());
    }

    /// <inheritdoc />
    public IWorkerBuilder UseMiddleware<TMiddleware>()
        where TMiddleware : class, IJobMiddleware
    {
        _services.AddScoped<TMiddleware>();
        _middlewareTypes.Add(typeof(TMiddleware));
        return this;
    }

    /// <inheritdoc />
    public IWorkerBuilder Use(Func<IJobContext, JobMiddlewareDelegate, Task> middleware)
    {
        _inlineMiddlewares.Add(middleware);
        return this;
    }

    private static bool IsJobHandler(Type type)
    {
        return typeof(IJobHandler).IsAssignableFrom(type) ||
               type.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IJobHandler<>));
    }
}
