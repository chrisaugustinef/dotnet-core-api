using System;
namespace VoyagesApi.Models
{
    public class CurrencyExchangeRates
    {
        public long Id { get; set; }
        public string BaseCurrency { get; set; }
        public string TargetCurrency { get; set; }
        public decimal ExchangeRate { get; set; }
    }
}