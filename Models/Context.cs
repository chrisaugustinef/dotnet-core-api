using Microsoft.EntityFrameworkCore;

namespace VoyagesApi.Models
{
    public class VoyagesContext : DbContext
    {
        public VoyagesContext (DbContextOptions<VoyagesContext> options)
            : base(options)
        {
        }

        public DbSet<Voyages> Voyages { get; set; }
        public DbSet<CurrencyExchangeRates> CurrencyExchangeRates { get; set; }
    }

    // public class CurrencyExchangeRatesContext : DbContext
    // {
    //     public CurrencyExchangeRatesContext (DbContextOptions<CurrencyExchangeRatesContext> options)
    //         : base(options)
    //     {
    //     }

    //     public DbSet<CurrencyExchangeRates> CurrencyExchangeRates { get; set; }
    // }
}
