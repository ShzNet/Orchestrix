using Microsoft.EntityFrameworkCore;
using Orchestrix.Coordinator.Persistence.Entities;

namespace Orchestrix.Coordinator.Persistence.EfCore.Stores;

/// <summary>
/// Implementation of coordinator node persistence using Entity Framework Core.
/// </summary>
public class CoordinatorNodeStore(CoordinatorDbContext context) : ICoordinatorNodeStore
{
    /// <inheritdoc />
    public async Task UpdateHeartbeatAsync(CoordinatorNodeEntity node, CancellationToken cancellationToken = default)
    {
        var existing = await context.CoordinatorNodes.FindAsync(new object[] { node.NodeId }, cancellationToken);
        if (existing == null)
        {
            context.CoordinatorNodes.Add(node);
        }
        else
        {
            existing.LastHeartbeat = node.LastHeartbeat;
            existing.Status = node.Status;
            // Don't override Role or JobCount here blindly unless intended? 
            // Usually heartbeat updates liveness. 
            // But if the node object passed in has fresh state, use it.
            existing.Metadata = node.Metadata;
            existing.Status = node.Status;
        }
        await context.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task UpdateRoleAsync(string nodeId, CoordinatorRole role, CancellationToken cancellationToken = default)
    {
        var node = await context.CoordinatorNodes.FindAsync(new object[] { nodeId }, cancellationToken);
        if (node != null)
        {
            node.Role = role;
            await context.SaveChangesAsync(cancellationToken);
        }
    }

    /// <inheritdoc />
    public async Task IncrementJobCountAsync(string nodeId, CancellationToken cancellationToken = default)
    {
        var node = await context.CoordinatorNodes.FindAsync(new object[] { nodeId }, cancellationToken);
        if (node != null)
        {
            node.JobCount++;
            await context.SaveChangesAsync(cancellationToken);
        }
    }

    /// <inheritdoc />
    public async Task DecrementJobCountAsync(string nodeId, CancellationToken cancellationToken = default)
    {
         var node = await context.CoordinatorNodes.FindAsync(new object[] { nodeId }, cancellationToken);
        if (node != null)
        {
            node.JobCount = Math.Max(0, node.JobCount - 1);
            await context.SaveChangesAsync(cancellationToken);
        }
    }

    /// <inheritdoc />
    public async Task MarkDrainingAsync(string nodeId, CancellationToken cancellationToken = default)
    {
        var node = await context.CoordinatorNodes.FindAsync(new object[] { nodeId }, cancellationToken);
        if (node != null)
        {
            node.Status = NodeStatus.Draining;
            await context.SaveChangesAsync(cancellationToken);
        }
    }

    /// <inheritdoc />
    public async Task RemoveNodeAsync(string nodeId, CancellationToken cancellationToken = default)
    {
        var node = await context.CoordinatorNodes.FindAsync(new object[] { nodeId }, cancellationToken);
        if (node != null)
        {
            context.CoordinatorNodes.Remove(node);
            await context.SaveChangesAsync(cancellationToken);
        }
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<CoordinatorNodeEntity>> GetDeadNodesAsync(TimeSpan timeout, CancellationToken cancellationToken = default)
    {
        var threshold = DateTimeOffset.UtcNow.Subtract(timeout);
        return await context.CoordinatorNodes
             .Where(n => n.Status != NodeStatus.Offline && n.LastHeartbeat < threshold)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<CoordinatorNodeEntity>> GetActiveFollowersAsync(CancellationToken cancellationToken = default)
    {
        return await context.CoordinatorNodes
            .Where(n => n.Status == NodeStatus.Active && n.Role == CoordinatorRole.Follower)
            .OrderBy(n => n.JobCount)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<CoordinatorNodeEntity>> GetAllActiveNodesAsync(CancellationToken cancellationToken = default)
    {
        return await context.CoordinatorNodes
            .Where(n => n.Status == NodeStatus.Active)
            .ToListAsync(cancellationToken);
    }
}
