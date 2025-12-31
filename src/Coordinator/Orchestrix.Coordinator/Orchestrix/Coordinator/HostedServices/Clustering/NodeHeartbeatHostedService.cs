using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Orchestrix.Coordinator.Persistence.Entities;
using Orchestrix.Coordinator.Persistence;

namespace Orchestrix.Coordinator.HostedServices.Clustering;

/// <summary>
/// Background service that sends heartbeats to the persistence store to indicate liveness.
/// </summary>
public class NodeHeartbeatHostedService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly CoordinatorOptions _options;
    private readonly ILogger<NodeHeartbeatHostedService> _logger;
    private readonly string _nodeId;

    /// <summary>
    /// Initializes a new instance of <see cref="NodeHeartbeatHostedService"/>.
    /// </summary>
    /// <param name="scopeFactory">The service scope factory.</param>
    /// <param name="options">The coordinator options.</param>
    /// <param name="logger">The logger.</param>
    public NodeHeartbeatHostedService(
        IServiceScopeFactory scopeFactory,
        CoordinatorOptions options,
        ILogger<NodeHeartbeatHostedService> logger)
    {
        _scopeFactory = scopeFactory;
        _options = options;
        _logger = logger;
        _nodeId = options.NodeId;
    }

    /// <inheritdoc />
    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting NodeHeartbeatHostedService for Node {NodeId}", _nodeId);
        await base.StartAsync(cancellationToken);
    }

    /// <inheritdoc />
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("NodeHeartbeatHostedService started. Heartbeat interval: {Interval}", _options.HeartbeatInterval);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var nodeStore = scope.ServiceProvider.GetRequiredService<ICoordinatorNodeStore>();

                var node = new CoordinatorNodeEntity
                {
                    NodeId = _nodeId,
                    Status = NodeStatus.Active,
                    LastHeartbeat = DateTimeOffset.UtcNow,
                    Hostname = Environment.MachineName,
                    ProcessId = System.Diagnostics.Process.GetCurrentProcess().Id,
                    Metadata = "{}" 
                };

                await nodeStore.UpdateHeartbeatAsync(node, stoppingToken);
                _logger.LogDebug("Heartbeat updated for Node {NodeId}", _nodeId);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending heartbeat for Node {NodeId}", _nodeId);
            }

            try
            {
                await Task.Delay(_options.HeartbeatInterval, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
        }
    }

    /// <inheritdoc />
    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        await base.StopAsync(cancellationToken);
        
        _logger.LogInformation("Stopping NodeHeartbeatHostedService for Node {NodeId}", _nodeId);
        try
        {
            _logger.LogInformation("Marking node {NodeId} as Offline (Graceful Shutdown)", _nodeId);
            
            // Create a NEW scope for shutdown to ensure we have a fresh DbContext
            // and aren't using a potentially disposed one.
            using var scope = _scopeFactory.CreateScope();
            var nodeStore = scope.ServiceProvider.GetRequiredService<ICoordinatorNodeStore>();

            // Ensure we are not a leader when shutting down
            await nodeStore.UpdateRoleAsync(_nodeId, CoordinatorRole.Follower, cancellationToken);

            var node = new CoordinatorNodeEntity
            {
                NodeId = _nodeId,
                Status = NodeStatus.Offline,
                LastHeartbeat = DateTimeOffset.UtcNow
            };
            
            // Use the cancellationToken from StopAsync to ensure we respect shutdown timeout
            await nodeStore.UpdateHeartbeatAsync(node, cancellationToken);
        }
        catch (Exception ex)
        {
             _logger.LogWarning(ex, "Failed to mark node as Offline during shutdown for Node {NodeId}", _nodeId);
        }
    }
}
