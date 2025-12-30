using WexChallenge.Application.DTOs;

namespace WexChallenge.Application.Services;

/// <summary>
/// Application service interface for purchase operations.
/// </summary>
public interface IPurchaseService
{
    /// <summary>
    /// Creates a new purchase transaction.
    /// </summary>
    /// <param name="request">The purchase creation request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created purchase response.</returns>
    Task<PurchaseResponse> CreatePurchaseAsync(CreatePurchaseRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a purchase by its ID.
    /// </summary>
    /// <param name="id">The purchase ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The purchase response if found.</returns>
    Task<PurchaseResponse> GetPurchaseByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a purchase converted to a specified currency.
    /// </summary>
    /// <param name="id">The purchase ID.</param>
    /// <param name="currency">The target currency for conversion.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The converted purchase response.</returns>
    Task<ConvertedPurchaseResponse> GetPurchaseInCurrencyAsync(Guid id, string currency, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all purchases.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of all purchases.</returns>
    Task<IReadOnlyList<PurchaseResponse>> GetAllPurchasesAsync(CancellationToken cancellationToken = default);
}
