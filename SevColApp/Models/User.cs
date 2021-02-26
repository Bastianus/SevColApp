using System.ComponentModel.DataAnnotations.Schema;

namespace SevColApp.Models
{
    public class User
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Prefixes { get; set; }
        public byte[] PasswordHash { get; set; }

        [NotMapped]
        public string FullName => string.IsNullOrEmpty(Prefixes) ? $"{FirstName} {LastName}" : $"{FirstName} {Prefixes} {LastName}";
        [NotMapped]
        public string Password { get; set; }
    }
}
