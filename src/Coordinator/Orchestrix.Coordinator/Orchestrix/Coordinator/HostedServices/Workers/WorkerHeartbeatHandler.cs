using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Orchestrix.Persistence;
using Orchestrix.Persistence.Entities;
using Orchestrix.Transport;
using Orchestrix.Transport.Messages.Workers;

namespace Orchestrix.Coordinator.HostedServices.Workers;

/// <summary>
/// Service that monitors worker heartbeats and updates their status in the database.
/// </summary>
public class WorkerHeartbeatHandler : BackgroundService
{
    private readonly ISubscriber _subscriber;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly TransportChannels _channels;
    private readonly ILogger<WorkerHeartbeatHandler> _logger;

    /// <summary>
    /// Initializes a new instance of <see cref="WorkerHeartbeatHandler"/>.
    /// </summary>
    public WorkerHeartbeatHandler(
        ISubscriber subscriber,
        IServiceScopeFactory scopeFactory,
        TransportChannels channels,
        ILogger<WorkerHeartbeatHandler> logger)
    {
        _subscriber = subscriber;
        _scopeFactory = scopeFactory;
        _channels = channels;
        _logger = logger;
    }

    /// <inheritdoc />
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("[WorkerHeartbeat] Starting worker heartbeat handler");

        await _subscriber.SubscribeAsync<WorkerMetricsMessage>(
            _channels.WorkerMetricsBroadcast,
            async message =>
            {
                await HandleWorkerMetricsAsync(message, stoppingToken);
                return true; // Continue subscription
            },
            stoppingToken);
    }

    private async Task HandleWorkerMetricsAsync(WorkerMetricsMessage metrics, CancellationToken cancellationToken)
    {
        _logger.LogDebug(
            "[WorkerHeartbeat] Received heartbeat from worker {WorkerId}. ActiveJobs: {ActiveJobs}/{MaxConcurrent}",
            metrics.WorkerId, metrics.ActiveJobs, metrics.MaxConcurrentJobs);

        try
        {
            using var scope = _scopeFactory.CreateScope();
            var workerStore = scope.ServiceProvider.GetRequiredService<IWorkerStore>();

            // Map status string to WorkerStatus enum
            var status = metrics.Status switch
            {
                "Running" => WorkerStatus.Active,
                "Draining" => WorkerStatus.Draining,
                _ => WorkerStatus.Offline
            };

            var workerEntity = new WorkerEntity
            {
                WorkerId = metrics.WorkerId,
                Queues = metrics.Queues,
                MaxConcurrency = metrics.MaxConcurrentJobs,
                CurrentLoad = metrics.ActiveJobs,
                LastHeartbeat = metrics.Timestamp,
                Status = status
            };

            await workerStore.UpdateHeartbeatAsync(workerEntity, cancellationToken);

            _logger.LogDebug(
                "[WorkerHeartbeat] Updated worker {WorkerId} heartbeat in database",
                metrics.WorkerId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[WorkerHeartbeat] Failed to update heartbeat for worker {WorkerId}", metrics.WorkerId);
        }
    }
}
