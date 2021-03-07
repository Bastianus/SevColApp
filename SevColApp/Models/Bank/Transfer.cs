using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace SevColApp.Models
{
    public class Transfer
    {
        public int Id { get; set; }
        public string PayingAccountNumber { get; set; }
        public string ReceivingAccountNumber { get; set; }
        public int Amount { get; set; }
        public DateTime Time { get; set; }
        [NotMapped]
        public List<string> Errors { get; set; }
    }
}
