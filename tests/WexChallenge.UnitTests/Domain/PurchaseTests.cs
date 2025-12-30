using FluentAssertions;
using WexChallenge.Domain.Entities;

namespace WexChallenge.UnitTests.Domain;

public class PurchaseTests
{
    [Fact]
    public void Create_WithValidData_ShouldCreatePurchase()
    {
        // Arrange
        var description = "Test Purchase";
        var transactionDate = DateTime.UtcNow.Date;
        var amount = 100.50m;

        // Act
        var purchase = Purchase.Create(description, transactionDate, amount);

        // Assert
        purchase.Should().NotBeNull();
        purchase.Id.Should().NotBe(Guid.Empty);
        purchase.Description.Should().Be(description);
        purchase.TransactionDate.Should().Be(transactionDate);
        purchase.AmountInUsd.Should().Be(amount);
        purchase.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void Create_WithAmountNeedingRounding_ShouldRoundToNearestCent()
    {
        // Arrange
        var amount = 100.555m;

        // Act
        var purchase = Purchase.Create("Test", DateTime.UtcNow, amount);

        // Assert
        purchase.AmountInUsd.Should().Be(100.56m);
    }

    [Fact]
    public void Create_ShouldStripTimeFromTransactionDate()
    {
        // Arrange
        var dateTimeWithTime = new DateTime(2024, 1, 15, 14, 30, 45);

        // Act
        var purchase = Purchase.Create("Test", dateTimeWithTime, 100m);

        // Assert
        purchase.TransactionDate.Should().Be(new DateTime(2024, 1, 15));
        purchase.TransactionDate.TimeOfDay.Should().Be(TimeSpan.Zero);
    }
}
