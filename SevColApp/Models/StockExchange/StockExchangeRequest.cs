using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace SevColApp.Models
{
    public class StockExchangeRequest
    {
        public int Id { get; set; }
        public uint NumberOfStocks { get; set; }
        public int userId { get; set; }
        public virtual User User { get; set; }
        public int companyId { get; set; }
        public virtual Company Company { get; set; }
        [NotMapped]
        public string Password { get; set; }
        [NotMapped]
        public List<string> Errors { get; set; }
        [NotMapped]
        public string CompanyName { get; set; }

        public StockExchangeRequest()
        {
            Errors = new List<string>();
        }
    }
}
