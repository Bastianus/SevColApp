using System.Collections.Generic;

namespace SevColApp.Models
{
    public class BuyRequestInputOutput
    {
        public StockExchangeBuyRequest BuyRequest { get; set; }
        public List<Company> Companies { get; set; }
        public List<BankAccount> BankAccounts { get; set; }
    }
}
