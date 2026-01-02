namespace Orchestrix.Worker.Registry;

/// <summary>
/// Interface for registering and resolving job handlers.
/// </summary>
public interface IJobHandlerRegistry
{
    /// <summary>
    /// Gets the handler type for a given job type name.
    /// </summary>
    /// <param name="jobType">The job type name.</param>
    /// <returns>The handler type, or null if not found.</returns>
    Type? GetHandlerType(string jobType);

    /// <summary>
    /// Gets the arguments type for a given job type name.
    /// </summary>
    /// <param name="jobType">The job type name.</param>
    /// <returns>The arguments type, or null if not found.</returns>
    Type? GetArgumentsType(string jobType);

    /// <summary>
    /// Registers a handler for a job type.
    /// </summary>
    /// <typeparam name="THandler">The handler type.</typeparam>
    /// <typeparam name="TArgs">The arguments type.</typeparam>
    /// <param name="jobType">The job type name.</param>
    void Register<THandler, TArgs>(string jobType)
        where THandler : class;

    /// <summary>
    /// Registers a handler for a job type without typed arguments.
    /// </summary>
    /// <typeparam name="THandler">The handler type.</typeparam>
    /// <param name="jobType">The job type name.</param>
    void Register<THandler>(string jobType)
        where THandler : class;
}
