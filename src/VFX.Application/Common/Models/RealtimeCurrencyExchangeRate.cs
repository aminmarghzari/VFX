namespace VFX.Application.Common.Models;

public class RealtimeCurrencyExchangeRate
{
    public string FromCurrencyCode { get; set; }
    public string FromCurrencyName { get; set; }
    public string ToCurrencyCode { get; set; }
    public string ToCurrencyName { get; set; }
    public decimal ExchangeRate { get; set; }
    public DateTime LastRefreshed { get; set; }
    public string TimeZone { get; set; }
    public decimal BidPrice { get; set; }
    public decimal AskPrice { get; set; }
}
