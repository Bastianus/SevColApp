using System.Collections.Generic;

namespace SevColApp.Models
{
    public class SellRequestInputOutput
    {
        public UsersCurrentStocks UsersCurrentStocks { get; set; }
        public StockExchangeSellRequest SellRequest { get; set; }
        public List<BankAccount> BankAccounts { get; set; }
    }
}
