using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Orchestrix.Locking;

namespace Orchestrix.Coordinator.LeaderElection;

/// <summary>
/// Implementation of distributed leader election using distributed locks.
/// </summary>
internal class LeaderElection(
    IDistributedLockProvider lockProvider,
    IOptions<CoordinatorOptions> options,
    ILogger<LeaderElection> logger)
    : ILeaderElection
{
    private const string LeaderLockKey = "orchestrix:coordinator:leader";

    private readonly CoordinatorOptions _options = options.Value;

    private CancellationTokenSource? _cts;
    private IDistributedLock? _leaderLock;
    private Task? _renewalTask;
    private volatile bool _isLeader;

    public event EventHandler<bool>? LeadershipChanged;

    public bool IsLeader => _isLeader;

    private void SetLeaderStatus(bool isLeader)
    {
        if (_isLeader == isLeader) return;
        _isLeader = isLeader;
        LeadershipChanged?.Invoke(this, isLeader);
    }

    public async Task StartAsync(CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Starting leader election for node {NodeId}", _options.NodeId);

        // Create linked cancellation token source
        _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

        // Start background task for leader election
        _renewalTask = RunElectionLoopAsync(_cts.Token);

        await Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Stopping leader election for node {NodeId}", _options.NodeId);

        // Cancel background task
        _cts?.Cancel();

        // Wait for renewal task to complete
        if (_renewalTask != null)
        {
            try
            {
                await _renewalTask;
            }
            catch (OperationCanceledException)
            {
                // Expected
            }
        }

        // Release lock if held
        if (_leaderLock != null)
        {
            try
            {
                await _leaderLock.DisposeAsync();
                _leaderLock = null;
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Error releasing leader lock");
            }
        }

        // Dispose cancellation token source
        _cts?.Dispose();
        _cts = null;

        SetLeaderStatus(false);
    }

    private async Task RunElectionLoopAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                // Try to acquire leader lock
                _leaderLock = lockProvider.CreateLock(LeaderLockKey);

                if (await _leaderLock.TryAcquireAsync(_options.LeaderLeaseDuration, cancellationToken))
                {
                    SetLeaderStatus(true);
                    logger.LogInformation("Node {NodeId} became leader", _options.NodeId);

                    // Renew lock periodically
                    await RenewLockAsync(cancellationToken);
                }
                else
                {
                    // Failed to acquire lock, retry after interval
                    logger.LogDebug("Node {NodeId} failed to acquire leader lock, retrying...", _options.NodeId);
                    await Task.Delay(_options.LeaderRenewInterval, cancellationToken);
                }
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in leader election loop");
                await Task.Delay(_options.LeaderRenewInterval, cancellationToken);
            }
        }
    }

    private async Task RenewLockAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested && _isLeader)
        {
            try
            {
                await Task.Delay(_options.LeaderRenewInterval, cancellationToken);

                if (_leaderLock == null)
                {
                    SetLeaderStatus(false);
                    logger.LogWarning("Leader lock is null, stepping down");
                    break;
                }

                // Extend the lock TTL
                var extended = await _leaderLock.ExtendAsync(_options.LeaderLeaseDuration, cancellationToken);
                if (!extended)
                {
                    SetLeaderStatus(false);
                    logger.LogWarning("Node {NodeId} failed to extend leader lock, stepping down", _options.NodeId);
                    break;
                }

                logger.LogDebug("Node {NodeId} extended leader lock", _options.NodeId);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error extending leader lock");
                SetLeaderStatus(false);
                break;
            }
        }

        // If we lost leadership, release the lock and retry
        if (_leaderLock != null)
        {
            try
            {
                await _leaderLock.DisposeAsync();
                _leaderLock = null;
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Error releasing leader lock after stepping down");
            }
        }
    }
}
