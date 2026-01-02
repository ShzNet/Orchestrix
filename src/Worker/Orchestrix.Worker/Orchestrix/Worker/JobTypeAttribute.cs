namespace Orchestrix.Worker;

/// <summary>
/// Attribute to specify the job type name for a handler.
/// Used by assembly scanning for auto-discovery.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class JobTypeAttribute : Attribute
{
    /// <summary>
    /// Gets the job type name.
    /// </summary>
    public string JobType { get; }

    /// <summary>
    /// Initializes a new instance of <see cref="JobTypeAttribute"/>.
    /// </summary>
    /// <param name="jobType">The job type name.</param>
    public JobTypeAttribute(string jobType)
    {
        JobType = jobType;
    }
}
