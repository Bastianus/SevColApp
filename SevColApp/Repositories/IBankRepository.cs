using SevColApp.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SevColApp.Repositories
{
    public interface IBankRepository
    {
        Task<List<BankAccount>> GetBankAccountsOfUser(int userId);
        Task<BankAccount> GetBankAccountById(int id);
        Task<List<Bank>> GetAllBanks();
        void CreateNewAccount(InputOutputAccountCreate input, int userId);
        Transfer ExecuteTransfer(Transfer transfer);
        Task<bool> IsAccountPasswordCorrect(string accountNumber, string password);
    }
}
