using Orchestrix.Coordinator.Persistence.Entities;

namespace Orchestrix.Coordinator.Persistence;

/// <summary>
/// Repository interface for dead letter operations.
/// Focused on failed job tracking and retry.
/// </summary>
public interface IDeadLetterStore
{
    /// <summary>
    /// Moves a failed job to dead letter queue.
    /// </summary>
    Task AddToDeadLetterAsync(DeadLetterEntity deadLetter, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all dead letter jobs (for admin UI/retry).
    /// </summary>
    Task<IReadOnlyList<DeadLetterEntity>> GetAllDeadLettersAsync(int skip = 0, int take = 100, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a specific dead letter job by ID (for retry).
    /// </summary>
    Task<DeadLetterEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes a dead letter job after manual retry.
    /// </summary>
    Task RemoveAsync(Guid id, CancellationToken cancellationToken = default);
}
