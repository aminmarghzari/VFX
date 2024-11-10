namespace VFX.Application.Services.Validation
{
    public static class CurrencyExchangeRateValidation
    {
        public static void ValidateCurrencyCodes(string fromCurrencyCode, string toCurrencyCode)
        {
            if (string.IsNullOrWhiteSpace(fromCurrencyCode))
                throw new ArgumentException("From currency code cannot be null or empty.", nameof(fromCurrencyCode));

            if (string.IsNullOrWhiteSpace(toCurrencyCode))
                throw new ArgumentException("To currency code cannot be null or empty.", nameof(toCurrencyCode));

            fromCurrencyCode = fromCurrencyCode.ToUpperInvariant();
            toCurrencyCode = toCurrencyCode.ToUpperInvariant();

            if (fromCurrencyCode == toCurrencyCode)
            {
                throw new ArgumentException($"From currency code ({fromCurrencyCode}) and to currency code ({toCurrencyCode}) cannot be the same.");
            }
        }
    }
}
