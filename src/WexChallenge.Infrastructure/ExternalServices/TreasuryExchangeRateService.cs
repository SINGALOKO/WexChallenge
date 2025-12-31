using System.Globalization;
using System.Net.Http.Json;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using WexChallenge.Domain.Interfaces;
using WexChallenge.Domain.ValueObjects;

namespace WexChallenge.Infrastructure.ExternalServices;

/// <summary>
/// Implementation of the exchange rate service using the Treasury Reporting Rates of Exchange API.
/// Includes caching to minimize external API calls for historical rates.
/// </summary>
public class TreasuryExchangeRateService : IExchangeRateService
{
    private readonly HttpClient _httpClient;
    private readonly IMemoryCache _cache;
    private readonly ILogger<TreasuryExchangeRateService> _logger;

    private const string BaseUrl = "https://api.fiscaldata.treasury.gov/services/api/fiscal_service/v1/accounting/od/rates_of_exchange";
    private const int SixMonthsInDays = 180; // Approximately 6 months
    private static readonly TimeSpan CacheDuration = TimeSpan.FromHours(24);

    public TreasuryExchangeRateService(
        HttpClient httpClient,
        IMemoryCache cache,
        ILogger<TreasuryExchangeRateService> logger)
    {
        _httpClient = httpClient;
        _cache = cache;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<ExchangeRate?> GetExchangeRateAsync(string currency, DateTime purchaseDate, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"exchange_rate_{currency}_{purchaseDate:yyyy-MM-dd}";

        if (_cache.TryGetValue(cacheKey, out ExchangeRate? cachedRate))
        {
            _logger.LogDebug("Cache hit for exchange rate: {CacheKey}", cacheKey);
            return cachedRate;
        }

        var sixMonthsAgo = purchaseDate.AddDays(-SixMonthsInDays);

        // Build the API query
        // The Treasury API uses "country_currency_desc" for combined country-currency (e.g., "Brazil-Real")
        // or "currency" for just the currency name (e.g., "Real")
        // We'll try country_currency_desc first as it's more specific
        var filter = $"country_currency_desc:eq:{Uri.EscapeDataString(currency)}," +
                     $"record_date:lte:{purchaseDate:yyyy-MM-dd}," +
                     $"record_date:gte:{sixMonthsAgo:yyyy-MM-dd}";

        var url = $"{BaseUrl}?filter={filter}&sort=-record_date&page[size]=1";

        _logger.LogInformation("Fetching exchange rate from Treasury API: {Url}", url);

        try
        {
            var response = await _httpClient.GetFromJsonAsync<TreasuryApiResponse>(url, cancellationToken);

            // If no results found with country_currency_desc, try with just currency field
            if (response?.Data is null || response.Data.Count == 0)
            {
                _logger.LogDebug("No results with country_currency_desc, trying currency field for: {Currency}", currency);
                filter = $"currency:eq:{Uri.EscapeDataString(currency)}," +
                         $"record_date:lte:{purchaseDate:yyyy-MM-dd}," +
                         $"record_date:gte:{sixMonthsAgo:yyyy-MM-dd}";
                url = $"{BaseUrl}?filter={filter}&sort=-record_date&page[size]=1";
                response = await _httpClient.GetFromJsonAsync<TreasuryApiResponse>(url, cancellationToken);
            }

            if (response?.Data is null || response.Data.Count == 0)
            {
                _logger.LogWarning("No exchange rate found for currency {Currency} within 6 months of {Date}",
                    currency, purchaseDate);
                return null;
            }

            var rateData = response.Data.First();

            if (!decimal.TryParse(rateData.ExchangeRate, CultureInfo.InvariantCulture, out var rate))
            {
                _logger.LogError("Failed to parse exchange rate value: {Rate}", rateData.ExchangeRate);
                return null;
            }

            // Use record_date as the effective date (the date the rate was published)
            if (!DateTime.TryParse(rateData.RecordDate, CultureInfo.InvariantCulture, DateTimeStyles.None, out var effectiveDate))
            {
                _logger.LogError("Failed to parse record date: {Date}", rateData.RecordDate);
                return null;
            }

            var exchangeRate = new ExchangeRate(
                rateData.Country,
                rateData.Currency,
                rate,
                effectiveDate);

            // Cache the result (historical rates don't change)
            _cache.Set(cacheKey, exchangeRate, CacheDuration);

            _logger.LogInformation("Found exchange rate for {Currency}: {Rate} (record date {Date})",
                currency, rate, effectiveDate);

            return exchangeRate;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Error fetching exchange rate from Treasury API");
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<string>> GetAvailableCurrenciesAsync(CancellationToken cancellationToken = default)
    {
        const string cacheKey = "available_currencies";

        if (_cache.TryGetValue(cacheKey, out IReadOnlyList<string>? cachedCurrencies) && cachedCurrencies is not null)
        {
            return cachedCurrencies;
        }

        // Get distinct country_currency_desc from recent data (this is what users should use)
        var url = $"{BaseUrl}?fields=country_currency_desc&page[size]=1000&sort=-record_date";

        _logger.LogInformation("Fetching available currencies from Treasury API");

        try
        {
            var response = await _httpClient.GetFromJsonAsync<TreasuryApiResponse>(url, cancellationToken);

            var currencies = response?.Data
                .Select(d => d.CountryCurrencyDesc)
                .Where(c => !string.IsNullOrEmpty(c))
                .Distinct()
                .OrderBy(c => c)
                .ToList() ?? new List<string>();

            _cache.Set(cacheKey, (IReadOnlyList<string>)currencies, TimeSpan.FromHours(24));

            return currencies;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Error fetching available currencies from Treasury API");
            throw;
        }
    }
}