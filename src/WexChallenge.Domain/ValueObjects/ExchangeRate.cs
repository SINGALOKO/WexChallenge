namespace WexChallenge.Domain.ValueObjects;

/// <summary>
/// Represents an exchange rate from a specific currency to USD.
/// </summary>
public sealed class ExchangeRate
{
    /// <summary>
    /// The country name associated with this currency.
    /// </summary>
    public string Country { get; }

    /// <summary>
    /// The currency name.
    /// </summary>
    public string Currency { get; }

    /// <summary>
    /// The exchange rate value (units of foreign currency per 1 USD).
    /// </summary>
    public decimal Rate { get; }

    /// <summary>
    /// The effective date of this exchange rate.
    /// </summary>
    public DateTime EffectiveDate { get; }

    public ExchangeRate(string country, string currency, decimal rate, DateTime effectiveDate)
    {
        if (string.IsNullOrWhiteSpace(country))
            throw new ArgumentException("Country cannot be null or empty.", nameof(country));

        if (string.IsNullOrWhiteSpace(currency))
            throw new ArgumentException("Currency cannot be null or empty.", nameof(currency));

        if (rate <= 0)
            throw new ArgumentException("Exchange rate must be positive.", nameof(rate));

        Country = country;
        Currency = currency;
        Rate = rate;
        EffectiveDate = effectiveDate.Date;
    }

    /// <summary>
    /// Converts a USD amount to this currency.
    /// </summary>
    /// <param name="usdAmount">Amount in USD.</param>
    /// <returns>Converted amount rounded to 2 decimal places.</returns>
    public decimal ConvertFromUsd(decimal usdAmount)
    {
        return Math.Round(usdAmount * Rate, 2, MidpointRounding.AwayFromZero);
    }
}
