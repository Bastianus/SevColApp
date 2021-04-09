using System.Collections.Generic;

namespace SevColApp.Models
{
    public class AddStocksInputOutput
    {
        public List<Company> Companies { get; set; }
        public List<User> Users { get; set; }
        public UserCompanyStocks UserCompanyStocks { get; set; }
        public int NumberOfStocks { get; set; }
        public List<string> Errors { get; set; }
    }
}
