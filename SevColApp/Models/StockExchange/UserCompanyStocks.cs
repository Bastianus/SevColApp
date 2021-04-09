namespace SevColApp.Models
{
    public class UserCompanyStocks
    {
        public int Id { get; set; }
        public uint NumberOfStocks { get; set; }
        public int userId { get; set; }
        public virtual User User { get; set; }
        public int companyId { get; set; }
        public virtual Company Company { get; set; }
    }
}
