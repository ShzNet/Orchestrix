namespace Orchestrix.Coordinator;

/// <summary>
/// Coordinator node status enumeration.
/// </summary>
public enum NodeStatus
{
    /// <summary>
    /// Node is active and healthy.
    /// </summary>
    Active,

    /// <summary>
    /// Node is draining (finishing current work, not accepting new jobs).
    /// </summary>
    Draining,

    /// <summary>
    /// Node is offline/dead.
    /// </summary>
    Offline
}
