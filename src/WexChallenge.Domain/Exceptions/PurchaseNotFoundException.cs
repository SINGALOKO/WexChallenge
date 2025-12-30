namespace WexChallenge.Domain.Exceptions;

/// <summary>
/// Exception thrown when a purchase is not found.
/// </summary>
public class PurchaseNotFoundException : DomainException
{
    public Guid PurchaseId { get; }

    public PurchaseNotFoundException(Guid purchaseId)
        : base("PURCHASE_NOT_FOUND", $"Purchase with ID '{purchaseId}' was not found.")
    {
        PurchaseId = purchaseId;
    }
}
