namespace SevColApp.Models
{
    public class UserWithExtraData
    {
        public User User { get; set; }
        public long TotalWealth { get; set; }
        public long TotalNumberOfStocks { get; set; }
    }
}
