using System.Collections.Generic;

namespace SevColApp.Models
{
    public class UsersCurrentStocks
    {
        public List<UserStocks> UserStocks { get; set; }

        public UsersCurrentStocks()
        {
            UserStocks = new List<UserStocks>();
        }
    }
}
