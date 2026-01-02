using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Orchestrix.Transport;
using Orchestrix.Transport.Messages.Jobs;
using Orchestrix.Worker.Execution;
using Orchestrix.Worker.Registration;

namespace Orchestrix.Worker.Consumer;

/// <summary>
/// Background service that consumes jobs from dispatch channels.
/// Waits for registration to complete before starting.
/// </summary>
public class JobConsumerService : BackgroundService
{
    private readonly ISubscriber _subscriber;
    private readonly TransportChannels _channels;
    private readonly IJobExecutor _executor;
    private readonly WorkerOptions _options;
    private readonly WorkerRegistrationService _registrationService;
    private readonly SemaphoreSlim _concurrencyLimiter;
    private readonly ILogger<JobConsumerService> _logger;

    private volatile int _activeJobs;

    /// <summary>
    /// Gets the current number of active jobs being executed.
    /// </summary>
    public int ActiveJobs => _activeJobs;

    /// <summary>
    /// Initializes a new instance of <see cref="JobConsumerService"/>.
    /// </summary>
    public JobConsumerService(
        ISubscriber subscriber,
        TransportChannels channels,
        IJobExecutor executor,
        WorkerOptions options,
        WorkerRegistrationService registrationService,
        ILogger<JobConsumerService> logger)
    {
        _subscriber = subscriber;
        _channels = channels;
        _executor = executor;
        _options = options;
        _registrationService = registrationService;
        _concurrencyLimiter = new SemaphoreSlim(options.MaxConcurrentJobs);
        _logger = logger;
    }

    /// <inheritdoc />
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Wait for registration to complete
        _logger.LogDebug("[JobConsumer] Waiting for registration to complete...");
        await _registrationService.RegistrationCompleteTask;

        _logger.LogInformation(
            "[JobConsumer] Starting consumer for worker {WorkerId}, queues: [{Queues}], max concurrent: {MaxConcurrent}",
            _options.WorkerId, string.Join(", ", _options.Queues), _options.MaxConcurrentJobs);

        var subscriptionTasks = new List<Task>();

        foreach (var queue in _options.Queues)
        {
            var channel = _channels.JobDispatch(queue);
            var task = SubscribeToQueueAsync(channel, queue, stoppingToken);
            subscriptionTasks.Add(task);

            _logger.LogInformation("[JobConsumer] Subscribed to channel {Channel}", channel);
        }

        await Task.WhenAll(subscriptionTasks);
    }

    private async Task SubscribeToQueueAsync(string channel, string queue, CancellationToken stoppingToken)
    {
        await _subscriber.SubscribeWithGroupAsync<JobDispatchMessage>(
            channel,
            "workers", // Consumer group name
            _options.WorkerId,
            async message =>
            {
                if (stoppingToken.IsCancellationRequested)
                    return false;

                // Wait for concurrency slot
                await _concurrencyLimiter.WaitAsync(stoppingToken);

                try
                {
                    Interlocked.Increment(ref _activeJobs);

                    _logger.LogDebug(
                        "[JobConsumer] Received job {JobId} from queue {Queue}, active jobs: {ActiveJobs}",
                        message.JobId, queue, _activeJobs);

                    // Execute job (fire and forget within semaphore)
                    _ = ExecuteJobAsync(message, stoppingToken);

                    return true; // Continue subscription
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "[JobConsumer] Error processing job {JobId}", message.JobId);
                    return true; // Continue subscription even on error
                }
            },
            stoppingToken);
    }

    private async Task ExecuteJobAsync(JobDispatchMessage message, CancellationToken stoppingToken)
    {
        try
        {
            await _executor.ExecuteAsync(message, stoppingToken);
        }
        finally
        {
            Interlocked.Decrement(ref _activeJobs);
            _concurrencyLimiter.Release();
        }
    }

    /// <inheritdoc />
    public override void Dispose()
    {
        _concurrencyLimiter.Dispose();
        base.Dispose();
    }
}
