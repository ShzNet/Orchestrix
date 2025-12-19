using Microsoft.Extensions.Hosting;

namespace Orchestrix.Coordinator.LeaderElection;

/// <summary>
/// Hosted service that manages the leader election lifecycle.
/// </summary>
internal class LeaderElectionHostedService(ILeaderElection leaderElection) : IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken)
    {
        return leaderElection.StartAsync(cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return leaderElection.StopAsync(cancellationToken);
    }
}
