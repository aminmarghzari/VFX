using System.Text.Json;
using VFX.Application.Common;
using VFX.Application.Common.Models;
using VFX.Application.Interface;
using VFX.Application.Services.Validation;

namespace VFX.Application.Services;

public class AlphavantageApiService : IAlphavantageApiService
{
    private readonly HttpClient _httpClient;
    private readonly CurrencyExchangeRateProvider _settings;

    public AlphavantageApiService(HttpClient httpClient, AppSettings appSettings)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _settings = appSettings?.CurrencyExchangeRateProvider ?? throw new ArgumentNullException(nameof(appSettings));
    }

    public async Task<RealtimeCurrencyExchangeRate> GetExchangeRateFromApi(string fromCurrencyCode, string toCurrencyCode)
    {
        CurrencyExchangeRateValidation.ValidateCurrencyCodes(fromCurrencyCode, toCurrencyCode);

        string queryUrl = $"{_settings.CurrencyExchangeRateApiUrl}&from_currency={fromCurrencyCode}&to_currency={toCurrencyCode}&apikey={_settings.ApiKey}";
        var response = await _httpClient.GetAsync(queryUrl);

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception("Failed to retrieve exchange rate from external API.");
        }

        var content = await response.Content.ReadAsStringAsync();
        var jsonData = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(content);

        if (jsonData != null && jsonData.TryGetValue("Realtime Currency Exchange Rate", out var exchangeRateInfo))
        {
            return ParseExchangeRate(exchangeRateInfo);
        }
        else
        {
            throw new Exception("Error: Unable to retrieve exchange rate information.");
        }
    }

    private RealtimeCurrencyExchangeRate ParseExchangeRate(JsonElement exchangeRateInfo)
    {
        return new RealtimeCurrencyExchangeRate
        {
            FromCurrencyCode = exchangeRateInfo.GetProperty("1. From_Currency Code").GetString() ?? "N/A",
            FromCurrencyName = exchangeRateInfo.GetProperty("2. From_Currency Name").GetString() ?? "N/A",
            ToCurrencyCode = exchangeRateInfo.GetProperty("3. To_Currency Code").GetString() ?? "N/A",
            ToCurrencyName = exchangeRateInfo.GetProperty("4. To_Currency Name").GetString() ?? "N/A",
            ExchangeRate = decimal.TryParse(exchangeRateInfo.GetProperty("5. Exchange Rate").GetString(), out var rate) ? rate : 0,
            LastRefreshed = DateTime.TryParse(exchangeRateInfo.GetProperty("6. Last Refreshed").GetString(), out var lastRefreshed) ? lastRefreshed : DateTime.MinValue,
            TimeZone = exchangeRateInfo.GetProperty("7. Time Zone").GetString() ?? "N/A",
            BidPrice = decimal.TryParse(exchangeRateInfo.GetProperty("8. Bid Price").GetString(), out var bidPrice) ? bidPrice : 0,
            AskPrice = decimal.TryParse(exchangeRateInfo.GetProperty("9. Ask Price").GetString(), out var askPrice) ? askPrice : 0
        };
    }
}
