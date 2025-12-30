using System.ComponentModel.DataAnnotations;

namespace WexChallenge.Application.DTOs;

/// <summary>
/// Request DTO for creating a new purchase transaction.
/// </summary>
public record CreatePurchaseRequest
{
    /// <summary>
    /// Description of the purchase (max 50 characters).
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string Description { get; init; } = string.Empty;

    /// <summary>
    /// Date when the transaction occurred.
    /// </summary>
    [Required]
    public DateTime TransactionDate { get; init; }

    /// <summary>
    /// Purchase amount in United States dollars (must be positive).
    /// </summary>
    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be a positive value.")]
    public decimal AmountInUsd { get; init; }
}
