using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace VoyagesApi.Models
{
    public class Voyages
    {
        //[Key]
        //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public string VoyageCode { get; set; }
        public decimal Price { get; set; }
        public string Currency { get; set; }
        public DateTimeOffset Timestamp { get; set; }
    }
    public enum Currency
    {
        USD,
        GBP,
        EUR
    }
}