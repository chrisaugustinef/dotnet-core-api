using System.Linq;
using VoyagesApi.Models;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("dotnet-core-api-tests")]
namespace VoyagesApi.Utils
{
    public class CurrencyUtils : ICurrencyUtils
    {
        public CurrencyUtils() {}

        public decimal ExchangeCurrency(string baseCode, string targetCode, decimal amount, List<CurrencyExchangeRates> currencyExchangeRates)
        {
            return amount * GetKnownRate(baseCode, targetCode, currencyExchangeRates);
        }

        internal decimal GetKnownRate(string baseCode, string targetCode, List<CurrencyExchangeRates> currencyExchangeRates)
        {
            var rate = currencyExchangeRates.SingleOrDefault(fr => fr.BaseCurrency == baseCode && fr.TargetCurrency == targetCode);
            var rate_i = currencyExchangeRates.SingleOrDefault(fr => fr.BaseCurrency == targetCode && fr.TargetCurrency == baseCode);
            if (rate == null)
                return 1 / rate_i.ExchangeRate;
            return rate.ExchangeRate;
        }

    }
}