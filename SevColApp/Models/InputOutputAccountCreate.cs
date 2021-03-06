using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SevColApp.Models
{
    public class InputOutputAccountCreate
    {
        [Required]
        public string AccountName { get; set; }
        [Required]
        public string BankName { get; set; }
        public bool UserHasAccount { get; set; }
        [Required]
        public string WealthLevel { get; set; }
        [Required]
        public string Password { get; set; }
        public List<Bank> Banks { get; set; }
        [NotMapped]
        public List<string> Errors { get; set; }
    }
}
