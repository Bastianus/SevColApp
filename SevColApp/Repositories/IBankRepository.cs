using SevColApp.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SevColApp.Repositories
{
    public interface IBankRepository
    {
        Task<List<BankAccount>> GetBankAccountsOfUser(int userId);
    }
}
