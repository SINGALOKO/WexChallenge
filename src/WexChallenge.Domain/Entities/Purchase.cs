namespace WexChallenge.Domain.Entities;

/// <summary>
/// Represents a purchase transaction stored in the system.
/// </summary>
public class Purchase
{
    /// <summary>
    /// Unique identifier for the purchase transaction.
    /// </summary>
    public Guid Id { get; private set; }

    /// <summary>
    /// Description of the purchase (max 50 characters).
    /// </summary>
    public string Description { get; private set; } = string.Empty;

    /// <summary>
    /// Date when the transaction occurred.
    /// </summary>
    public DateTime TransactionDate { get; private set; }

    /// <summary>
    /// Purchase amount in United States dollars, rounded to the nearest cent.
    /// </summary>
    public decimal AmountInUsd { get; private set; }

    /// <summary>
    /// Timestamp when the record was created.
    /// </summary>
    public DateTime CreatedAt { get; private set; }

    // Private constructor for EF Core
    private Purchase() { }

    /// <summary>
    /// Creates a new purchase transaction.
    /// </summary>
    /// <param name="description">Description of the purchase (max 50 characters).</param>
    /// <param name="transactionDate">Date of the transaction.</param>
    /// <param name="amountInUsd">Amount in USD (must be positive).</param>
    /// <returns>A new Purchase instance.</returns>
    public static Purchase Create(string description, DateTime transactionDate, decimal amountInUsd)
    {
        return new Purchase
        {
            Id = Guid.NewGuid(),
            Description = description,
            TransactionDate = transactionDate.Date,
            AmountInUsd = Math.Round(amountInUsd, 2, MidpointRounding.AwayFromZero),
            CreatedAt = DateTime.UtcNow
        };
    }
}
