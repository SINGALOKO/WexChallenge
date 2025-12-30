using FluentAssertions;
using WexChallenge.Domain.ValueObjects;

namespace WexChallenge.UnitTests.Domain;

public class ExchangeRateTests
{
    [Fact]
    public void Constructor_WithValidData_ShouldCreateExchangeRate()
    {
        // Arrange
        var country = "Brazil";
        var currency = "Real";
        var rate = 5.50m;
        var effectiveDate = new DateTime(2024, 1, 15);

        // Act
        var exchangeRate = new ExchangeRate(country, currency, rate, effectiveDate);

        // Assert
        exchangeRate.Country.Should().Be(country);
        exchangeRate.Currency.Should().Be(currency);
        exchangeRate.Rate.Should().Be(rate);
        exchangeRate.EffectiveDate.Should().Be(effectiveDate.Date);
    }

    [Fact]
    public void Constructor_WithEmptyCountry_ShouldThrowArgumentException()
    {
        // Act
        var act = () => new ExchangeRate("", "Real", 5.50m, DateTime.Now);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithParameterName("country");
    }

    [Fact]
    public void Constructor_WithEmptyCurrency_ShouldThrowArgumentException()
    {
        // Act
        var act = () => new ExchangeRate("Brazil", "", 5.50m, DateTime.Now);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithParameterName("currency");
    }

    [Fact]
    public void Constructor_WithZeroRate_ShouldThrowArgumentException()
    {
        // Act
        var act = () => new ExchangeRate("Brazil", "Real", 0m, DateTime.Now);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithParameterName("rate");
    }

    [Fact]
    public void Constructor_WithNegativeRate_ShouldThrowArgumentException()
    {
        // Act
        var act = () => new ExchangeRate("Brazil", "Real", -5.50m, DateTime.Now);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithParameterName("rate");
    }

    [Fact]
    public void ConvertFromUsd_ShouldReturnCorrectAmount()
    {
        // Arrange
        var exchangeRate = new ExchangeRate("Brazil", "Real", 5.50m, DateTime.Now);
        var usdAmount = 100m;

        // Act
        var result = exchangeRate.ConvertFromUsd(usdAmount);

        // Assert
        result.Should().Be(550.00m);
    }

    [Fact]
    public void ConvertFromUsd_ShouldRoundToTwoDecimalPlaces()
    {
        // Arrange
        var exchangeRate = new ExchangeRate("Brazil", "Real", 5.555m, DateTime.Now);
        var usdAmount = 100m;

        // Act
        var result = exchangeRate.ConvertFromUsd(usdAmount);

        // Assert
        result.Should().Be(555.50m);
    }
}
