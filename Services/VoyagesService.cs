using VoyagesApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System;
using VoyagesApi.Utils;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Memory;

namespace VoyagesApi.Services
{
    public class VoyagesService : IVoyagesService
    {
        private readonly VoyagesContext _context;
        private readonly ICurrencyUtils _currencyUtils;
        public VoyagesService(VoyagesContext context,
                                ICurrencyUtils currencyUtils)
        {
            _context = context;
            _currencyUtils = currencyUtils;

            //Creating in-memory table for Voyages
            if (_context.Voyages.Count() == 0)
            {

                _context.Voyages.Add(new Voyages { Id = 1, VoyageCode = "400S", Price = 35.9M, Currency = Currency.EUR.ToString(), Timestamp = DateTimeOffset.Now });
                _context.Voyages.Add(new Voyages { Id = 2, VoyageCode = "401S", Price = 45.9M, Currency = Currency.USD.ToString(), Timestamp = DateTimeOffset.Now });
                _context.Voyages.Add(new Voyages { Id = 3, VoyageCode = "402S", Price = 55.9M, Currency = Currency.USD.ToString(), Timestamp = DateTimeOffset.Now });
                _context.Voyages.Add(new Voyages { Id = 4, VoyageCode = "403S", Price = 65.9M, Currency = Currency.EUR.ToString(), Timestamp = DateTimeOffset.Now });
                _context.Voyages.Add(new Voyages { Id = 5, VoyageCode = "404S", Price = 75.9M, Currency = Currency.USD.ToString(), Timestamp = DateTimeOffset.Now });
                _context.Voyages.Add(new Voyages { Id = 6, VoyageCode = "405S", Price = 85.9M, Currency = Currency.USD.ToString(), Timestamp = DateTimeOffset.Now });
                _context.Voyages.Add(new Voyages { Id = 7, VoyageCode = "406S", Price = 95.9M, Currency = Currency.USD.ToString(), Timestamp = DateTimeOffset.Now });
                _context.Voyages.Add(new Voyages { Id = 8, VoyageCode = "407S", Price = 15.9M, Currency = Currency.EUR.ToString(), Timestamp = DateTimeOffset.Now });
                _context.Voyages.Add(new Voyages { Id = 9, VoyageCode = "408S", Price = 38.9M, Currency = Currency.GBP.ToString(), Timestamp = DateTimeOffset.Now });
                _context.Voyages.Add(new Voyages { Id = 10, VoyageCode = "409S", Price = 34.9M, Currency = Currency.USD.ToString(), Timestamp = DateTimeOffset.Now });
                _context.SaveChanges();
            }

            //Creating in-memory table for Currency Exchange Rates
            if (_context.CurrencyExchangeRates.Count() == 0)
            {

                _context.CurrencyExchangeRates.Add(new CurrencyExchangeRates { Id = 1, BaseCurrency = Currency.USD.ToString(), TargetCurrency = Currency.GBP.ToString(), ExchangeRate = 0.76M });
                _context.CurrencyExchangeRates.Add(new CurrencyExchangeRates { Id = 2, BaseCurrency = Currency.USD.ToString(), TargetCurrency = Currency.EUR.ToString(), ExchangeRate = 0.91M });
                _context.CurrencyExchangeRates.Add(new CurrencyExchangeRates { Id = 3, BaseCurrency = Currency.GBP.ToString(), TargetCurrency = Currency.EUR.ToString(), ExchangeRate = 1.21M });
                _context.SaveChanges();
            }
        }

        public async Task<IEnumerable<Voyages>> GetVoyages()
        {
            return await _context.Voyages.ToListAsync();
        }

        public async Task<IEnumerable<CurrencyExchangeRates>> GetCurrencyExchangeRates()
        {
            return await _context.CurrencyExchangeRates.ToListAsync();
        }

        public async Task<decimal> GetAverage(string voyageCode, string currency)
        {
            var voyage = await _context.Voyages.Where(x => x.VoyageCode == voyageCode).FirstOrDefaultAsync();
            return _currencyUtils.ExchangeCurrency(voyage.Currency, currency, voyage.Price, _context.CurrencyExchangeRates.ToList());
        }

        public async Task UpdatePrice(Voyages voyagesItem)
        {
            _context.Entry(voyagesItem).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public bool VoyagesItemExists(long id)
        {
            return _context.Voyages.Any(e => e.Id == id);
        }
    }
}