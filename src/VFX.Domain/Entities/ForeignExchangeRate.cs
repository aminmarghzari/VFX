using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VFX.Application.Common.Models;

namespace VFX.Domain.Entities;

// Represents a foreign exchange rate record in the system
public class ForeignExchangeRate : BaseModel
{
    // The ID of the source currency (e.g., USD)
    [Required]
    public int FromCurrencyId { get; set; }

    // Navigation property to the source currency entity (e.g., USD)
    [ForeignKey("FromCurrencyId")]
    public Currency FromCurrency { get; set; }

    // The ID of the target currency (e.g., EUR)
    [Required]
    public int ToCurrencyId { get; set; }

    // Navigation property to the target currency entity (e.g., EUR)
    [ForeignKey("ToCurrencyId")]
    public Currency ToCurrency { get; set; }

    // The bid price of the currency pair (the price a buyer is willing to pay)
    public decimal BidPrice { get; set; }

    // The ask price of the currency pair (the price a seller is asking)
    public decimal AskPrice { get; set; }

    // The exchange rate between the two currencies
    public decimal ExchangeRate { get; set; }

    // The last time the exchange rate was refreshed or updated
    public DateTime LastRefreshed { get; set; }

    // The timezone in which the exchange rate was recorded
    public string TimeZone { get; set; } = string.Empty;
}
