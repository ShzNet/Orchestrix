using Microsoft.EntityFrameworkCore;
using Orchestrix.Persistence.Entities;

namespace Orchestrix.Persistence.EfCore.Stores;

/// <summary>
/// Implementation of dead letter persistence using Entity Framework Core.
/// </summary>
public class DeadLetterStore(CoordinatorDbContext context) : IDeadLetterStore
{
    /// <inheritdoc />
    public async Task AddToDeadLetterAsync(DeadLetterEntity deadLetter, CancellationToken cancellationToken = default)
    {
        context.DeadLetters.Add(deadLetter);
        await context.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<DeadLetterEntity>> GetAllDeadLettersAsync(int skip = 0, int take = 100, CancellationToken cancellationToken = default)
    {
        return await context.DeadLetters
            .OrderByDescending(d => d.FailedAt)
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<DeadLetterEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await context.DeadLetters.FindAsync(new object[] { id }, cancellationToken);
    }

    /// <inheritdoc />
    public async Task RemoveAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var deadLetter = await context.DeadLetters.FindAsync(new object[] { id }, cancellationToken);
        if (deadLetter != null)
        {
            context.DeadLetters.Remove(deadLetter);
            await context.SaveChangesAsync(cancellationToken);
        }
    }
}
