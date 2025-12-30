using WexChallenge.Domain.Entities;

namespace WexChallenge.Domain.Interfaces;

/// <summary>
/// Repository interface for Purchase entity operations.
/// </summary>
public interface IPurchaseRepository
{
    /// <summary>
    /// Adds a new purchase to the repository.
    /// </summary>
    /// <param name="purchase">The purchase to add.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task AddAsync(Purchase purchase, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a purchase by its unique identifier.
    /// </summary>
    /// <param name="id">The purchase ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The purchase if found, null otherwise.</returns>
    Task<Purchase?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all purchases.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of all purchases.</returns>
    Task<IReadOnlyList<Purchase>> GetAllAsync(CancellationToken cancellationToken = default);
}
