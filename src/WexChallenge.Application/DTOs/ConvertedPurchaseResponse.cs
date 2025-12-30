namespace WexChallenge.Application.DTOs;

/// <summary>
/// Response DTO for a purchase converted to a target currency.
/// </summary>
public record ConvertedPurchaseResponse
{
    /// <summary>
    /// Unique identifier for the purchase.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Description of the purchase.
    /// </summary>
    public string Description { get; init; } = string.Empty;

    /// <summary>
    /// Date of the transaction.
    /// </summary>
    public DateTime TransactionDate { get; init; }

    /// <summary>
    /// Original purchase amount in USD.
    /// </summary>
    public decimal OriginalAmountInUsd { get; init; }

    /// <summary>
    /// The target currency used for conversion.
    /// </summary>
    public string TargetCurrency { get; init; } = string.Empty;

    /// <summary>
    /// The exchange rate used for conversion (units per 1 USD).
    /// </summary>
    public decimal ExchangeRate { get; init; }

    /// <summary>
    /// The date of the exchange rate used.
    /// </summary>
    public DateTime ExchangeRateDate { get; init; }

    /// <summary>
    /// The converted amount in the target currency.
    /// </summary>
    public decimal ConvertedAmount { get; init; }
}
