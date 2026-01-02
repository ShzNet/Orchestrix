using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Orchestrix.Transport;
using Orchestrix.Transport.Messages.Workers;

namespace Orchestrix.Worker.Registration;

/// <summary>
/// Service that registers the worker with the Coordinator on startup.
/// Must complete before other worker services start.
/// </summary>
public class WorkerRegistrationService : IHostedService
{
    private readonly IPublisher _publisher;
    private readonly ISubscriber _subscriber;
    private readonly TransportChannels _channels;
    private readonly WorkerOptions _options;
    private readonly WorkerRuntimeConfig _runtimeConfig;
    private readonly ILogger<WorkerRegistrationService> _logger;
    private readonly TaskCompletionSource<bool> _registrationComplete = new();

    /// <summary>
    /// Gets a task that completes when registration is done.
    /// </summary>
    public Task RegistrationCompleteTask => _registrationComplete.Task;

    /// <summary>
    /// Initializes a new instance of <see cref="WorkerRegistrationService"/>.
    /// </summary>
    public WorkerRegistrationService(
        IPublisher publisher,
        ISubscriber subscriber,
        TransportChannels channels,
        WorkerOptions options,
        WorkerRuntimeConfig runtimeConfig,
        ILogger<WorkerRegistrationService> logger)
    {
        _publisher = publisher;
        _subscriber = subscriber;
        _channels = channels;
        _options = options;
        _runtimeConfig = runtimeConfig;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "[WorkerRegistration] Starting registration for worker {WorkerId}",
            _options.WorkerId);

        // Subscribe to config response channel first
        var configChannel = _channels.WorkerConfig(_options.WorkerId);
        await _subscriber.SubscribeAsync<WorkerConfigMessage>(
            configChannel,
            async message =>
            {
                await HandleConfigResponseAsync(message);
                return false; // Unsubscribe after receiving config
            },
            cancellationToken);

        // Publish join message
        var joinMessage = new WorkerJoinMessage
        {
            WorkerId = _options.WorkerId,
            HostName = _options.WorkerName,
            Queues = _options.Queues,
            MaxConcurrency = _options.MaxConcurrentJobs,
            Timestamp = DateTimeOffset.UtcNow
        };

        await _publisher.PublishAsync(_channels.WorkerJoin, joinMessage, cancellationToken);
        _logger.LogInformation("[WorkerRegistration] Published join request to Coordinator");

        // Wait for response with timeout
        var timeout = TimeSpan.FromSeconds(30);
        using var timeoutCts = new CancellationTokenSource(timeout);
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutCts.Token);

        try
        {
            await Task.WhenAny(_registrationComplete.Task, Task.Delay(timeout, linkedCts.Token));

            if (!_registrationComplete.Task.IsCompleted)
            {
                _logger.LogWarning(
                    "[WorkerRegistration] Timeout waiting for Coordinator response. Using default configuration.");
                
                // Use defaults from WorkerOptions
                _runtimeConfig.HeartbeatInterval = _options.HeartbeatInterval;
                _runtimeConfig.DefaultJobTimeout = _options.DefaultJobTimeout;
                _runtimeConfig.OnConfigurationReceived();
            }
        }
        catch (OperationCanceledException) when (timeoutCts.IsCancellationRequested)
        {
            _logger.LogWarning("[WorkerRegistration] Registration timeout. Using defaults.");
            _runtimeConfig.HeartbeatInterval = _options.HeartbeatInterval;
            _runtimeConfig.DefaultJobTimeout = _options.DefaultJobTimeout;
            _runtimeConfig.OnConfigurationReceived();
        }

        // Cleanup subscription
        await _subscriber.UnsubscribeAsync(configChannel);
    }

    private Task HandleConfigResponseAsync(WorkerConfigMessage message)
    {
        if (!message.Accepted)
        {
            _logger.LogError(
                "[WorkerRegistration] Registration rejected: {Reason}",
                message.RejectionReason);
            _registrationComplete.TrySetException(
                new InvalidOperationException($"Worker registration rejected: {message.RejectionReason}"));
            return Task.CompletedTask;
        }

        _logger.LogInformation(
            "[WorkerRegistration] Received configuration from Coordinator. " +
            "HeartbeatInterval: {Heartbeat}, JobTimeout: {Timeout}",
            message.HeartbeatInterval, message.DefaultJobTimeout);

        // Update runtime config
        _runtimeConfig.HeartbeatInterval = message.HeartbeatInterval;
        _runtimeConfig.DefaultJobTimeout = message.DefaultJobTimeout;
        _runtimeConfig.WorkerTimeout = message.WorkerTimeout;
        _runtimeConfig.OnConfigurationReceived();

        _registrationComplete.TrySetResult(true);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
