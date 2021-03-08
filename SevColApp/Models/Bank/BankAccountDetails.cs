using System.Collections.Generic;

namespace SevColApp.Models
{
    public class BankAccountDetails
    {
        public int Id { get; set; }
        public string AccountName { get; set; }
        public string AccountNumber { get; set; }
        public long Credit { get; set; }
        public List<Transfer> Transfers { get; set; }
    }
}
