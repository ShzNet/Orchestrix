using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Orchestrix.Persistence;
using Orchestrix.Persistence.Entities;
using Orchestrix.Transport;
using Orchestrix.Transport.Messages.Workers;

namespace Orchestrix.Coordinator.HostedServices.Workers;

/// <summary>
/// Service that handles worker registration requests and sends configuration.
/// </summary>
public class WorkerRegistrationHandler : BackgroundService
{
    private readonly ISubscriber _subscriber;
    private readonly IPublisher _publisher;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly TransportChannels _channels;
    private readonly CoordinatorOptions _options;
    private readonly ILogger<WorkerRegistrationHandler> _logger;

    /// <summary>
    /// Initializes a new instance of <see cref="WorkerRegistrationHandler"/>.
    /// </summary>
    public WorkerRegistrationHandler(
        ISubscriber subscriber,
        IPublisher publisher,
        IServiceScopeFactory scopeFactory,
        TransportChannels channels,
        CoordinatorOptions options,
        ILogger<WorkerRegistrationHandler> logger)
    {
        _subscriber = subscriber;
        _publisher = publisher;
        _scopeFactory = scopeFactory;
        _channels = channels;
        _options = options;
        _logger = logger;
    }

    /// <inheritdoc />
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("[WorkerRegistration] Starting worker registration handler");

        await _subscriber.SubscribeAsync<WorkerJoinMessage>(
            _channels.WorkerJoin,
            async message =>
            {
                await HandleWorkerJoinAsync(message, stoppingToken);
                return true; // Continue subscription
            },
            stoppingToken);
    }

    private async Task HandleWorkerJoinAsync(WorkerJoinMessage join, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "[WorkerRegistration] Worker {WorkerId} ({HostName}) joining. Queues: [{Queues}], MaxConcurrency: {MaxConcurrency}",
            join.WorkerId, join.HostName, string.Join(", ", join.Queues), join.MaxConcurrency);

        // Create a new scope to resolve scoped services (DbContext)
        using var scope = _scopeFactory.CreateScope();
        var workerStore = scope.ServiceProvider.GetRequiredService<IWorkerStore>();

        // Persist worker info to database
        var workerEntity = new WorkerEntity
        {
            WorkerId = join.WorkerId,
            HostName = join.HostName,
            Queues = join.Queues,
            MaxConcurrency = join.MaxConcurrency,
            CurrentLoad = 0,
            LastHeartbeat = DateTimeOffset.UtcNow,
            Status = WorkerStatus.Active,
            RegisteredAt = DateTimeOffset.UtcNow,
            Metadata = join.Metadata.Count > 0 ? JsonSerializer.Serialize(join.Metadata) : null
        };

        await workerStore.UpdateHeartbeatAsync(workerEntity, cancellationToken);
        _logger.LogInformation("[WorkerRegistration] Persisted worker {WorkerId} to database", join.WorkerId);

        // Send configuration response
        var configMessage = new WorkerConfigMessage
        {
            WorkerId = join.WorkerId,
            Accepted = true,
            HeartbeatInterval = _options.DefaultWorkerHeartbeatInterval,
            DefaultJobTimeout = _options.DefaultWorkerJobTimeout,
            WorkerTimeout = _options.DefaultWorkerTimeout,
            Timestamp = DateTimeOffset.UtcNow
        };

        var configChannel = _channels.WorkerConfig(join.WorkerId);
        await _publisher.PublishAsync(configChannel, configMessage, cancellationToken);

        _logger.LogInformation(
            "[WorkerRegistration] Sent configuration to worker {WorkerId}. " +
            "HeartbeatInterval: {Heartbeat}, JobTimeout: {Timeout}",
            join.WorkerId, configMessage.HeartbeatInterval, configMessage.DefaultJobTimeout);
    }
}
