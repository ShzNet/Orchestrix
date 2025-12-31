using Orchestrix.Coordinator.Communication.Cluster;
using Orchestrix.Persistence;
using Orchestrix.Persistence.Entities;

namespace Orchestrix.Coordinator.Services.Monitoring;

/// <summary>
/// Implementation of <see cref="INodeMetricsCollector"/>.
/// Aggregates system metrics and persisted job statistics.
/// </summary>
public class NodeMetricsCollector(
    ISystemMetricsCollector systemMetrics,
    IJobStatsCache jobStats) : INodeMetricsCollector
{
    /// <inheritdoc />
    public Task<NodeMetricsMessage> CollectMetricsAsync(CoordinatorNodeEntity node, CancellationToken cancellationToken = default)
    {
        var cpuMillicores = systemMetrics.GetCpuMillicores();
        var memoryBytes = systemMetrics.GetMemoryUsageBytes();
        
        // Retrieve cached job counts (in-memory, fast)
        var monitoredJobCount = jobStats.MonitoredJobCount;
        var queuedJobCount = jobStats.QueuedJobCount;

        var message = new NodeMetricsMessage
        {
            NodeId = node.NodeId,
            Role = node.Role, 
            Status = node.Status,
            
            Timestamp = DateTimeOffset.UtcNow,
            Hostname = node.Hostname,
            ProcessId = node.ProcessId,
            Metadata = node.Metadata,
            
            CpuMillicores = cpuMillicores,
            MemoryUsageBytes = memoryBytes,
            JobCount = monitoredJobCount,
            QueuedJobCount = queuedJobCount
        };

        return Task.FromResult(message);
    }
}
