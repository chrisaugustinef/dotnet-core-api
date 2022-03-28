using VoyagesApi.Models;
using System.Collections.Generic;

namespace VoyagesApi.Utils
{
    public interface ICurrencyUtils
    {
        decimal ExchangeCurrency(string baseCode, string targetCode, decimal amount, List<CurrencyExchangeRates> currencyExchangeRates);
    }
}