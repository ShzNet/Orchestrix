namespace Orchestrix.Coordinator;

/// <summary>
/// Coordinator role enumeration.
/// </summary>
public enum CoordinatorRole
{
    /// <summary>
    /// Leader coordinator (handles scheduling and dispatching).
    /// </summary>
    Leader,

    /// <summary>
    /// Follower coordinator (handles job event processing).
    /// </summary>
    Follower
}
