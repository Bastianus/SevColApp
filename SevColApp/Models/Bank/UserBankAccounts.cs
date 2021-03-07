using System.Collections.Generic;

namespace SevColApp.Models
{
    public class UserBankAccounts
    {
        public User User { get; set; }
        public List<BankAccount> BankAccounts { get; set; }
    }
}
