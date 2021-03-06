using System.Collections.Generic;

namespace SevColApp.Models
{
    public class UserAccountsAnswer
    {
        public User User { get; set; }
        public List<BankAccount> Accounts { get; set; }
        public string Error { get; set; }
    }
}
