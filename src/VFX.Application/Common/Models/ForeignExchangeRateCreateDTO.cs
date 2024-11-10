﻿namespace VFX.Application.Common.Models;

public class ForeignExchangeRateCreateDTO
{
    public string FromCurrencyCode { get; set; }
    public string ToCurrencyCode { get; set; }
    public decimal ExchangeRate { get; set; }
    public string TimeZone { get; set; }
    public decimal BidPrice { get; set; }
    public decimal AskPrice { get; set; }
}