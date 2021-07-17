using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace SevColApp.Models
{
    public class Global
    {
        public int Id { get; set; }
        public float MarketTrendFactor { get; set; }
        public float MarketVolatility { get; set; }
        [NotMapped]
        public List<string> Errors { get; set; }
        [NotMapped]
        public string InputStringTrend { get; set; }
        [NotMapped]
        public string InputStringVolatility { get; set; }
    }
}
