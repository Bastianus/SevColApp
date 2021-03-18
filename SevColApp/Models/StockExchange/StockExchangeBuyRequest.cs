using System.ComponentModel.DataAnnotations.Schema;

namespace SevColApp.Models
{
    public class StockExchangeBuyRequest : StockExchangeRequest
    {
        public uint OfferPerStock { get; set; }
        [NotMapped]
        public int AmountToPay { get; set; }
    }
}
