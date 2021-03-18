using System;

namespace SevColApp.Models
{
    public class StockExchangeCompleted
    {
        public int Id { get; set; }
        public uint NumberOfStocks { get; set; }
        public uint AmountPerStock { get; set; }
        public int companyId { get; set; }
        public virtual Company Company { get; set; }
        public int sellerId { get; set; }
        public virtual User Seller { get; set; }
        public int buyerId { get; set; }
        public virtual User Buyer { get; set; }
        public DateTime ExchangeDateAndTime { get; set; }
    }
}
