using Orchestrix.Coordinator.Communication.Cluster;
using Orchestrix.Persistence.Entities;

namespace Orchestrix.Coordinator.Services.Monitoring;

/// <summary>
/// Service for collecting comprehensive node metrics, including system resources and job statistics.
/// </summary>
public interface INodeMetricsCollector
{
    /// <summary>
    /// Collects metrics for the specified node.
    /// </summary>
    /// <param name="node">The node entity (provides identification and current state).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A populated metrics message.</returns>
    Task<NodeMetricsMessage> CollectMetricsAsync(CoordinatorNodeEntity node, CancellationToken cancellationToken = default);
}
