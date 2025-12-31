using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Orchestrix.Coordinator.Services.Clustering;
using Orchestrix.Persistence;
using Orchestrix.Transport;


namespace Orchestrix.Coordinator.HostedServices.Clustering;

/// <summary>
/// Background service that monitors cluster health and cleans up dead nodes.
/// Runs only on the Leader node.
/// </summary>
public class ClusterHealthMonitorHostedService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILeaderElection _leaderElection;
    private readonly ILogger<ClusterHealthMonitorHostedService> _logger;

    // Check interval. Could be configurable, defaulting to heartbeat interval * 2.
    private readonly TimeSpan _checkInterval;
    // Timeout threshold. Defaulting to heartbeat interval * 3.
    private readonly TimeSpan _timeoutThreshold;

    /// <summary>
    /// Initializes a new instance of the <see cref="ClusterHealthMonitorHostedService"/> class.
    /// </summary>
    /// <param name="scopeFactory">The service scope factory.</param>
    /// <param name="leaderElection">The leader election service.</param>
    /// <param name="options">The coordinator options.</param>
    /// <param name="logger">The logger.</param>
    public ClusterHealthMonitorHostedService(
        IServiceScopeFactory scopeFactory,
        ILeaderElection leaderElection,
        CoordinatorOptions options,
        ILogger<ClusterHealthMonitorHostedService> logger)
    {
        _scopeFactory = scopeFactory;
        _leaderElection = leaderElection;
        _logger = logger;

        _checkInterval = options.HeartbeatInterval * 2;
        _timeoutThreshold = options.HeartbeatInterval * 3;
    }

    /// <inheritdoc />
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("ClusterHealthMonitorHostedService started. Check Interval: {Interval}, Timeout: {Timeout}", _checkInterval, _timeoutThreshold);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                // Only Leader performs cleanup to avoid race conditions
                if (_leaderElection.IsLeader)
                {
                    await CheckAndCleanupDeadNodesAsync(stoppingToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in ClusterHealthMonitor loop");
            }

            try
            {
                await Task.Delay(_checkInterval, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
        }
    }

    private async Task CheckAndCleanupDeadNodesAsync(CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var nodeStore = scope.ServiceProvider.GetRequiredService<ICoordinatorNodeStore>();

        var deadNodes = await nodeStore.GetDeadNodesAsync(_timeoutThreshold, cancellationToken);
        
        if (deadNodes.Count > 0)
        {
            _logger.LogWarning("Detected {Count} dead nodes", deadNodes.Count);

            foreach (var node in deadNodes)
            {
                try
                {
                    _logger.LogInformation("Cleaning up dead node {NodeId} (LastHeartbeat: {LastHeartbeat})", node.NodeId, node.LastHeartbeat);
                    
                    // 1. Mark as Offline
                    // Use UpdateHeartbeatAsync to update status. 
                    // We keep the original LastHeartbeat to preserve the time of death/last contact.
                    // Note: Ensure the store implementation respects the passed LastHeartbeat and doesn't overwrite it with UtcNow if provided.
                    // (Assuming store implementation allows updating just status if LastHeartbeat is provided).
                    // If store overwrites LastHeartbeat, we might lose the "death time", but effectively the node is offline.
                    
                    node.Status = NodeStatus.Offline;
                    await nodeStore.UpdateHeartbeatAsync(node, cancellationToken);
                    
                    // TODO: Handle unfinished jobs redistribution instead of publishing generic event.
                    // Logic: Get jobs assigned to this node -> Redispatch them or mark as Pending.
                }

                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to cleanup dead node {NodeId}", node.NodeId);
                }
            }
        }
    }
}
