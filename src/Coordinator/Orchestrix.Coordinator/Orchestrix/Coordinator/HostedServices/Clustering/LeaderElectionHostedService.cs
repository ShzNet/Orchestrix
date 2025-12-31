using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Orchestrix.Coordinator.Services.Clustering;

namespace Orchestrix.Coordinator.HostedServices.Clustering;

/// <summary>
/// Background service that continuously attempts to acquire or renew leadership.
/// </summary>
public class LeaderElectionHostedService : BackgroundService
{
    private readonly LeaderElection _leaderElection;
    private readonly CoordinatorOptions _options;
    private readonly ILogger<LeaderElectionHostedService> _logger;

    /// <summary>
    /// Initializes a new instance of <see cref="LeaderElectionHostedService"/>.
    /// </summary>
    /// <param name="leaderElection">The leader election service.</param>
    /// <param name="options">The coordinator options.</param>
    /// <param name="logger">The logger.</param>
    public LeaderElectionHostedService(
        LeaderElection leaderElection,
        CoordinatorOptions options,
        ILogger<LeaderElectionHostedService> logger)
    {
        _leaderElection = leaderElection;
        _options = options;
        _logger = logger;
    }

    /// <inheritdoc />
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("[LeaderElection] Starting election loop for Node {NodeId}", _options.NodeId);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                if (_leaderElection.IsLeader)
                {
                    // If already leader, renew
                    await _leaderElection.ExtendLeadershipAsync(stoppingToken); // Method handles logging warnings if fails
                }
                else
                {
                    // Try to become leader
                    await _leaderElection.TryAcquireLeadershipAsync(stoppingToken); // Method handles logging if succeeds
                }

                // If leader, sleep for renew interval. If not, sleep for retry interval (using same for now)
                await Task.Delay(_options.LeaderRenewInterval, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[LeaderElection] Critical error in election loop. Retrying in 5s.");
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }

        // Cleanup on shutdown
        await _leaderElection.ReleaseLeadershipAsync();
        _logger.LogInformation("[LeaderElection] Stopped.");
    }
}
