using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Orchestrix.Transport;
using Orchestrix.Transport.Messages.Workers;
using Orchestrix.Worker.Consumer;
using Orchestrix.Worker.Registration;

namespace Orchestrix.Worker.Heartbeat;

/// <summary>
/// Background service that sends periodic heartbeats to coordinator.
/// Waits for registration to complete before starting.
/// </summary>
public class HeartbeatService : BackgroundService
{
    private readonly IPublisher _publisher;
    private readonly TransportChannels _channels;
    private readonly WorkerOptions _options;
    private readonly WorkerRuntimeConfig _runtimeConfig;
    private readonly WorkerRegistrationService _registrationService;
    private readonly JobConsumerService _consumerService;
    private readonly ILogger<HeartbeatService> _logger;

    /// <summary>
    /// Initializes a new instance of <see cref="HeartbeatService"/>.
    /// </summary>
    public HeartbeatService(
        IPublisher publisher,
        TransportChannels channels,
        WorkerOptions options,
        WorkerRuntimeConfig runtimeConfig,
        WorkerRegistrationService registrationService,
        JobConsumerService consumerService,
        ILogger<HeartbeatService> logger)
    {
        _publisher = publisher;
        _channels = channels;
        _options = options;
        _runtimeConfig = runtimeConfig;
        _registrationService = registrationService;
        _consumerService = consumerService;
        _logger = logger;
    }

    /// <inheritdoc />
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Wait for registration to complete
        _logger.LogDebug("[Heartbeat] Waiting for registration to complete...");
        await _registrationService.RegistrationCompleteTask;

        _logger.LogInformation(
            "[Heartbeat] Starting heartbeat for worker {WorkerId}, interval: {Interval}",
            _options.WorkerId, _runtimeConfig.HeartbeatInterval);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await SendHeartbeatAsync(stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[Heartbeat] Error sending heartbeat");
            }

            await Task.Delay(_runtimeConfig.HeartbeatInterval, stoppingToken);
        }

        _logger.LogInformation("[Heartbeat] Stopped for worker {WorkerId}", _options.WorkerId);
    }

    private async Task SendHeartbeatAsync(CancellationToken stoppingToken)
    {
        var message = new WorkerMetricsMessage
        {
            WorkerId = _options.WorkerId,
            Status = WorkerState.Running.ToString(),
            Queues = _options.Queues,
            ActiveJobs = _consumerService.ActiveJobs,
            MaxConcurrentJobs = _options.MaxConcurrentJobs,
            Timestamp = DateTimeOffset.UtcNow
        };

        var channel = _channels.WorkerMetricsBroadcast;
        await _publisher.PublishAsync(channel, message, stoppingToken);

        _logger.LogDebug(
            "[Heartbeat] Sent heartbeat - ActiveJobs: {ActiveJobs}/{MaxConcurrent}",
            _consumerService.ActiveJobs, _options.MaxConcurrentJobs);
    }
}
