namespace SevColApp.Models
{
    public class StockExchangeCompleted
    {
        public int Id { get; set; }
        public int NumberOfStocks { get; set; }
        public int AmountPerStock { get; set; }
        public int companyId { get; set; }
        public virtual Company Company { get; set; }
        public int sellerId { get; set; }
        public virtual User Seller { get; set; }
        public int buyerId { get; set; }
        public virtual User Buyer { get; set; }
    }
}
