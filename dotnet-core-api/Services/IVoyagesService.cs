using VoyagesApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace VoyagesApi.Services
{
    public interface IVoyagesService
    {
        Task<IEnumerable<Voyages>> GetVoyages();
        Task<IEnumerable<CurrencyExchangeRates>> GetCurrencyExchangeRates();
        Task<decimal> GetAverage(string voyageCode, string currency);
        Task UpdatePrice(Voyages voyagesItem);
        bool VoyagesItemExists(long id);
    }
}