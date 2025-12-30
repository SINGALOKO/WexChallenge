using System.Text.Json.Serialization;

namespace WexChallenge.Infrastructure.ExternalServices;

/// <summary>
/// Response model from the Treasury Reporting Rates of Exchange API.
/// </summary>
public class TreasuryApiResponse
{
    [JsonPropertyName("data")]
    public List<ExchangeRateData> Data { get; set; } = new();
}

/// <summary>
/// Individual exchange rate record from the Treasury API.
/// </summary>
public class ExchangeRateData
{
    [JsonPropertyName("country")]
    public string Country { get; set; } = string.Empty;

    [JsonPropertyName("currency")]
    public string Currency { get; set; } = string.Empty;

    [JsonPropertyName("exchange_rate")]
    public string ExchangeRate { get; set; } = string.Empty;

    [JsonPropertyName("record_date")]
    public string RecordDate { get; set; } = string.Empty;

    [JsonPropertyName("effective_date")]
    public string EffectiveDate { get; set; } = string.Empty;
}
