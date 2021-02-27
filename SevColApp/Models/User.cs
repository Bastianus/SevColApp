using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SevColApp.Models
{
    public class User
    {
        public int Id { get; set; }
        [Required]
        public string LoginName { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        public string Prefixes { get; set; }
        [Required]
        public byte[] PasswordHash { get; set; }

        [NotMapped]
        public string FullName => string.IsNullOrEmpty(Prefixes) ? $"{FirstName} {LastName}" : $"{FirstName} {Prefixes} {LastName}";
        [NotMapped]
        public string Password { get; set; }
    }
}
