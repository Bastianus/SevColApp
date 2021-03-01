using System.Collections.Generic;

namespace SevColApp.Models
{
    public class InputOutputAccountCreate
    {
        public string AccountName { get; set; }
        public string BankName { get; set; }
        public bool UserHasAccount { get; set; }
        public string WealthLevel { get; set; }
        public string Password { get; set; }
        public List<Bank> Banks { get; set; }
    }
}
