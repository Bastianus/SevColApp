namespace SevColApp.Models
{
    public class StockExchangeSellRequest : StockExchangeRequest
    {
        public int MinimumPerStock { get; set; }
    }
}
