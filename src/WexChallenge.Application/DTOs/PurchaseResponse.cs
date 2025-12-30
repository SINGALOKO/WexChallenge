namespace WexChallenge.Application.DTOs;

/// <summary>
/// Response DTO for a purchase transaction.
/// </summary>
public record PurchaseResponse
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
    public decimal AmountInUsd { get; init; }
}
