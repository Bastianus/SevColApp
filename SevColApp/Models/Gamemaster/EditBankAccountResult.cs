using System.Collections.Generic;

namespace SevColApp.Models
{
    public class EditBankAccountResult
    {
        public string FromAccount { get; set; }
        public string ToAccount { get; set; }
        public long AmountChange { get; set; }
        public long NewAmount { get; set; }
        public List<string> Errors { get; set; }
    }
}
