using FluentAssertions;
using NSubstitute;
using WexChallenge.Application.DTOs;
using WexChallenge.Application.Services;
using WexChallenge.Domain.Entities;
using WexChallenge.Domain.Exceptions;
using WexChallenge.Domain.Interfaces;
using WexChallenge.Domain.ValueObjects;

namespace WexChallenge.UnitTests.Application;

public class PurchaseServiceTests
{
    private readonly IPurchaseRepository _purchaseRepository;
    private readonly IExchangeRateService _exchangeRateService;
    private readonly PurchaseService _sut;

    public PurchaseServiceTests()
    {
        _purchaseRepository = Substitute.For<IPurchaseRepository>();
        _exchangeRateService = Substitute.For<IExchangeRateService>();
        _sut = new PurchaseService(_purchaseRepository, _exchangeRateService);
    }

    [Fact]
    public async Task CreatePurchaseAsync_ShouldCreateAndReturnPurchase()
    {
        // Arrange
        var request = new CreatePurchaseRequest
        {
            Description = "Test Purchase",
            TransactionDate = DateTime.UtcNow.Date,
            AmountInUsd = 100.50m
        };

        // Act
        var result = await _sut.CreatePurchaseAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().NotBe(Guid.Empty);
        result.Description.Should().Be(request.Description);
        result.AmountInUsd.Should().Be(request.AmountInUsd);

        await _purchaseRepository.Received(1).AddAsync(Arg.Any<Purchase>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetPurchaseByIdAsync_WhenPurchaseExists_ShouldReturnPurchase()
    {
        // Arrange
        var purchase = Purchase.Create("Test", DateTime.UtcNow.Date, 100m);
        _purchaseRepository.GetByIdAsync(purchase.Id, Arg.Any<CancellationToken>())
            .Returns(purchase);

        // Act
        var result = await _sut.GetPurchaseByIdAsync(purchase.Id);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(purchase.Id);
        result.Description.Should().Be(purchase.Description);
    }

    [Fact]
    public async Task GetPurchaseByIdAsync_WhenPurchaseDoesNotExist_ShouldThrowPurchaseNotFoundException()
    {
        // Arrange
        var purchaseId = Guid.NewGuid();
        _purchaseRepository.GetByIdAsync(purchaseId, Arg.Any<CancellationToken>())
            .Returns((Purchase?)null);

        // Act
        var act = () => _sut.GetPurchaseByIdAsync(purchaseId);

        // Assert
        await act.Should().ThrowAsync<PurchaseNotFoundException>()
            .Where(ex => ex.PurchaseId == purchaseId);
    }

    [Fact]
    public async Task GetPurchaseInCurrencyAsync_WithValidExchangeRate_ShouldReturnConvertedPurchase()
    {
        // Arrange
        var purchase = Purchase.Create("Test", new DateTime(2024, 1, 15), 100m);
        var exchangeRate = new ExchangeRate("Brazil", "Real", 5.00m, new DateTime(2024, 1, 10));

        _purchaseRepository.GetByIdAsync(purchase.Id, Arg.Any<CancellationToken>())
            .Returns(purchase);
        _exchangeRateService.GetExchangeRateAsync("Real", purchase.TransactionDate, Arg.Any<CancellationToken>())
            .Returns(exchangeRate);

        // Act
        var result = await _sut.GetPurchaseInCurrencyAsync(purchase.Id, "Real");

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(purchase.Id);
        result.OriginalAmountInUsd.Should().Be(100m);
        result.ExchangeRate.Should().Be(5.00m);
        result.ConvertedAmount.Should().Be(500.00m);
        result.TargetCurrency.Should().Be("Real");
    }

    [Fact]
    public async Task GetPurchaseInCurrencyAsync_WhenExchangeRateNotAvailable_ShouldThrowExchangeRateUnavailableException()
    {
        // Arrange
        var purchase = Purchase.Create("Test", new DateTime(2024, 1, 15), 100m);

        _purchaseRepository.GetByIdAsync(purchase.Id, Arg.Any<CancellationToken>())
            .Returns(purchase);
        _exchangeRateService.GetExchangeRateAsync("InvalidCurrency", purchase.TransactionDate, Arg.Any<CancellationToken>())
            .Returns((ExchangeRate?)null);

        // Act
        var act = () => _sut.GetPurchaseInCurrencyAsync(purchase.Id, "InvalidCurrency");

        // Assert
        await act.Should().ThrowAsync<ExchangeRateUnavailableException>()
            .Where(ex => ex.Currency == "InvalidCurrency");
    }

    [Fact]
    public async Task GetPurchaseInCurrencyAsync_WhenPurchaseDoesNotExist_ShouldThrowPurchaseNotFoundException()
    {
        // Arrange
        var purchaseId = Guid.NewGuid();
        _purchaseRepository.GetByIdAsync(purchaseId, Arg.Any<CancellationToken>())
            .Returns((Purchase?)null);

        // Act
        var act = () => _sut.GetPurchaseInCurrencyAsync(purchaseId, "Real");

        // Assert
        await act.Should().ThrowAsync<PurchaseNotFoundException>();
    }

    [Fact]
    public async Task GetAllPurchasesAsync_ShouldReturnAllPurchases()
    {
        // Arrange
        var purchases = new List<Purchase>
        {
            Purchase.Create("Purchase 1", DateTime.UtcNow.Date, 100m),
            Purchase.Create("Purchase 2", DateTime.UtcNow.Date.AddDays(-1), 200m)
        };

        _purchaseRepository.GetAllAsync(Arg.Any<CancellationToken>())
            .Returns(purchases);

        // Act
        var result = await _sut.GetAllPurchasesAsync();

        // Assert
        result.Should().HaveCount(2);
        result.Select(p => p.Description).Should().Contain(new[] { "Purchase 1", "Purchase 2" });
    }
}
