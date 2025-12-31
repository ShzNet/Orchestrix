using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Orchestrix.Coordinator.HostedServices.Base
{
    /// <summary>
    /// Abstract base class for BackgroundServices that run on a periodic interval 
    /// and require a new IServiceScope for each execution.
    /// </summary>
    public abstract class IntervalHostedService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of <see cref="IntervalHostedService"/>.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        /// <param name="logger">The logger.</param>
        protected IntervalHostedService(IServiceProvider serviceProvider, ILogger logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        /// <inheritdoc />
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation($"{GetType().Name} is starting.");

            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    try
                    {
                        if (ShouldExecute())
                        {
                            await RunIntervalAsync(stoppingToken);
                        }
                    }
                    catch (OperationCanceledException)
                    {
                        // Graceful shutdown
                        break;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"{GetType().Name} failed during execution iteration.");
                    }

                    // Wait for next interval or cancellation
                    await Task.Delay(GetInterval(), stoppingToken);
                }
            }
            finally
            {
                _logger.LogInformation($"{GetType().Name} is stopping.");
            }
        }

        private async Task RunIntervalAsync(CancellationToken stoppingToken)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                await ExecuteScopedAsync(scope.ServiceProvider, stoppingToken);
            }
        }

        /// <summary>
        /// Execute the logic within a created ServiceScope.
        /// </summary>
        protected abstract Task ExecuteScopedAsync(IServiceProvider scopedProvider, CancellationToken stoppingToken);

        /// <summary>
        /// Define the delay between executions.
        /// </summary>
        protected abstract TimeSpan GetInterval();

        /// <summary>
        /// Optional hook to check preconditions before execution (e.g., verifying leadership).
        /// Defaults to true.
        /// </summary>
        protected virtual bool ShouldExecute() => true;
    }
}
