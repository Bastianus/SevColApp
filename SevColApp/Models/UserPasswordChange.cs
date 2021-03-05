using System.ComponentModel.DataAnnotations;

namespace SevColApp.Models
{
    public class UserPasswordChange
    {
        [Required]
        public string UserLoginName { get; set; }
        [Required]
        public string NewPassword { get; set; }
    }
}
