using System.ComponentModel.DataAnnotations;

namespace SevColApp.Models
{
    public class AccountPasswordChange
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        public string BankAccountName { get; set; }
        [Required]
        public string NewPassword { get; set; }
        public string Error { get; set; }
    }
}
