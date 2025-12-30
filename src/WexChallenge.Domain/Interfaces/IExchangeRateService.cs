using WexChallenge.Domain.ValueObjects;

namespace WexChallenge.Domain.Interfaces;

/// <summary>
/// Service interface for retrieving exchange rates from the Treasury API.
/// </summary>
public interface IExchangeRateService
{
    /// <summary>
    /// Gets the exchange rate for a specific currency and date.
    /// Returns the most recent rate within 6 months prior to or equal to the specified date.
    /// </summary>
    /// <param name="currency">Target currency name (e.g., "Brazil-Real", "Euro Zone-Euro").</param>
    /// <param name="purchaseDate">The purchase date for which to find the exchange rate.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The exchange rate if found within the 6-month window.</returns>
    Task<ExchangeRate?> GetExchangeRateAsync(string currency, DateTime purchaseDate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all available currencies.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of available currency names.</returns>
    Task<IReadOnlyList<string>> GetAvailableCurrenciesAsync(CancellationToken cancellationToken = default);
}
