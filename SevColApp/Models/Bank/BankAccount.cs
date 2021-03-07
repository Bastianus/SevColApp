using System.ComponentModel.DataAnnotations.Schema;

namespace SevColApp.Models
{
    public class BankAccount
    {
        public int Id { get; set; }
        public string AccountNumber { get; set; }
        public string AccountName { get; set; }
        public long Credit { get; set; }
        public long ExpectedIncome { get; set; }
        [NotMapped]
        public string Password { get; set; }
        public byte[] PasswordHash { get; set; }
        public int userId { get; set; }
        public virtual User user { get; set; }
        public int BankId { get; set; }
        public virtual Bank Bank { get; set; }
    }
}
