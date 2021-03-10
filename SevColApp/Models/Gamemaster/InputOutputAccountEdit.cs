using System.Collections.Generic;

namespace SevColApp.Models
{
    public class InputOutputAccountEdit
    {
        public List<string> Errors { get; set; }
        public int Amount { get; set; }        
        public bool WithTransfer { get; set; }
        public string AccountNumber { get; set; }
        public bool FromRandomSevColAccount { get; set; }
        public BankAccount Account { get; set; }
    }
}
