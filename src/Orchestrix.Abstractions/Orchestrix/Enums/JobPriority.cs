namespace Orchestrix.Enums;

/// <summary>
/// Represents the priority level of a job.
/// </summary>
public enum JobPriority
{
    /// <summary>
    /// Low priority - processed after normal priority jobs.
    /// </summary>
    Low = 0,

    /// <summary>
    /// Normal priority - default priority level.
    /// </summary>
    Normal = 1,

    /// <summary>
    /// High priority - processed before normal priority jobs.
    /// </summary>
    High = 2,

    /// <summary>
    /// Critical priority - processed immediately.
    /// </summary>
    Critical = 3
}
