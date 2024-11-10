using VFX.Application.Common.Models;
using VFX.Domain.Entities;

namespace VFX.Application.Services.Validation;

public static class ForeignExchangeRateValidation
{
    public static void ValidateCurrencyPair(string currencyPair)
    {
        if (string.IsNullOrWhiteSpace(currencyPair))
        {
            throw new ArgumentException("Currency pair cannot be null or empty.", nameof(currencyPair));
        }

        var currencies = currencyPair.Split('/');
        if (currencies.Length != 2)
        {
            throw new ArgumentException("Invalid currency pair format. Expected format: 'USD/EUR'.", nameof(currencyPair));
        }

        string fromCurrencyCode = currencies[0].ToUpperInvariant();
        string toCurrencyCode = currencies[1].ToUpperInvariant();

        if (fromCurrencyCode == toCurrencyCode)
        {
            throw new ArgumentException($"From currency code ({fromCurrencyCode}) and to currency code ({toCurrencyCode}) cannot be the same.");
        }
    }

    public static void ValidateExchangeRate(RealtimeCurrencyExchangeRate rate)
    {
        if (rate == null)
        {
            throw new ArgumentNullException(nameof(rate), "Exchange rate cannot be null.");
        }
    }

    public static void ValidateCurrency(Currency currency, string currencyCode)
    {
        if (currency == null)
        {
            throw new Exception($"Currency with code {currencyCode} does not exist.");
        }
    }
}
