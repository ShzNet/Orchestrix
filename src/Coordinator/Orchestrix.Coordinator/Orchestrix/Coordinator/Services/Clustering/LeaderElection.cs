using Microsoft.Extensions.Logging;
using Orchestrix.Locking;

namespace Orchestrix.Coordinator.Services.Clustering;

/// <summary>
/// Distributed leader election using lock provider.
/// </summary>
public class LeaderElection(
    IDistributedLockProvider lockProvider,
    CoordinatorOptions options,
    ILogger<LeaderElection> logger)
    : ILeaderElection
{
    private const string LockResource = "orchestrix:coordinator:leader";

    // State
    private IDistributedLock? _currentLock;
    private volatile bool _isLeader;
    
    /// <inheritdoc />
    public bool IsLeader => _isLeader;

    /// <inheritdoc />
    public event EventHandler<bool>? LeadershipChanged;

    /// <summary>
    /// Attempts to acquire the leader lock.
    /// </summary>
    public async Task<bool> TryAcquireLeadershipAsync(CancellationToken ct)
    {
        try
        {
            if (_isLeader && _currentLock != null)
            {
                // Already leader, try to extend
                return await ExtendLeadershipAsync(ct);
            }

            var lockHandle = lockProvider.CreateLock(LockResource, new DistributedLockOptions 
            { 
                DefaultTtl = options.LeaderLeaseDuration
            });

            logger.LogDebug("[LeaderElection] Trying to acquire leader lock: {Resource}", LockResource);
            if (await lockHandle.TryAcquireAsync(options.LeaderLeaseDuration, ct))
            {
                logger.LogInformation("[LeaderElection] Successfully acquired leadership for node {NodeId}", options.NodeId);
                _currentLock = lockHandle;
                SetLeadership(true);
                return true;
            }
            else
            {
                logger.LogDebug("[LeaderElection] Failed to acquire leader lock (already held). Node {NodeId} is Follower.", options.NodeId);
            }
        }
        catch (OperationCanceledException)
        {
            // Expected during shutdown
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "[LeaderElection] Error trying to acquire leadership.");
        }

        return false;
    }

    /// <summary>
    /// Attempts to extend the current leader lock.
    /// </summary>
    public async Task<bool> ExtendLeadershipAsync(CancellationToken ct)
    {
        if (_currentLock == null || !_isLeader) return false;

        try
        {
            logger.LogDebug("[LeaderElection] Extending leadership for node {NodeId}", options.NodeId);
            if (await _currentLock.ExtendAsync(options.LeaderLeaseDuration, ct))
            {
                logger.LogDebug("[LeaderElection] Leadership extended successfully.");
                return true;
            }
            
            logger.LogWarning("[LeaderElection] Failed to extend lock. NodeId: {NodeId}, LeaseDuration: {LeaseDuration}", options.NodeId, options.LeaderLeaseDuration);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "[LeaderElection] Error extending leadership.");
        }

        // Verify if we lost it
        SetLeadership(false);
        _currentLock = null;
        return false;
    }

    /// <summary>
    /// Releases the leader lock.
    /// </summary>
    public async Task ReleaseLeadershipAsync()
    {
        if (_currentLock != null)
        {
            try
            {
                await _currentLock.ReleaseAsync();
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "[LeaderElection] Error releasing lock.");
            }
            finally
            {
                _currentLock = null;
                SetLeadership(false);
            }
        }
    }

    private void SetLeadership(bool isLeader)
    {
        if (_isLeader != isLeader)
        {
            _isLeader = isLeader;
            logger.LogInformation("[LeaderElection] Leadership changed: {IsLeader}", isLeader);
            LeadershipChanged?.Invoke(this, isLeader);
        }
    }
}
