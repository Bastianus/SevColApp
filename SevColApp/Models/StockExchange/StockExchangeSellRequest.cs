using System.ComponentModel.DataAnnotations.Schema;

namespace SevColApp.Models
{
    public class StockExchangeSellRequest : StockExchangeRequest
    {
        public uint MinimumPerStock { get; set; }
        [NotMapped]
        public int AmountToBePaid { get; set; }
    }
}
