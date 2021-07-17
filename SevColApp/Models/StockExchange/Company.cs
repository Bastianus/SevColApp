using System.ComponentModel.DataAnnotations.Schema;

namespace SevColApp.Models
{
    public class Company
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int NumberOfStocks { get; set; }
        [NotMapped]
        public bool IsNew { get; set; }
        public float CompanyTrendFactor { get; set; }
        public float CompanyVolatility { get; set; }
    }
}
