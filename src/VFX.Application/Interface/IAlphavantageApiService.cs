using VFX.Application.Common.Models;

namespace VFX.Application.Interface;

public interface IAlphavantageApiService
{
    Task<RealtimeCurrencyExchangeRate> GetExchangeRateFromApi(string fromCurrencyCode, string toCurrencyCode);
}
