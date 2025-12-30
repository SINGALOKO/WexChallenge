namespace WexChallenge.Domain.Exceptions;

/// <summary>
/// Exception thrown when no valid exchange rate is available for currency conversion.
/// </summary>
public class ExchangeRateUnavailableException : DomainException
{
    public string Currency { get; }
    public DateTime PurchaseDate { get; }

    public ExchangeRateUnavailableException(string currency, DateTime purchaseDate)
        : base("EXCHANGE_RATE_UNAVAILABLE",
            $"The purchase cannot be converted to the target currency '{currency}'. " +
            $"No exchange rate is available within 6 months prior to the purchase date ({purchaseDate:yyyy-MM-dd}).")
    {
        Currency = currency;
        PurchaseDate = purchaseDate;
    }
}
