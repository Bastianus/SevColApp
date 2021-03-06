using SevColApp.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SevColApp.Repositories
{
    public interface IBankRepository
    {
        List<BankAccount> GetBankAccountsOfUser(int userId);
        BankAccount GetBankAccountById(int id);
        BankAccountDetails GetBankAccountDetailsByAccountName(string accountName);
        List<Bank> GetAllBanks();
        List<string> GetAllBankNames();
        List<string> GetAllBankAccountNumbers();
        void CreateNewAccount(InputOutputAccountCreate input, int userId);
        Transfer ExecuteTransfer(Transfer transfer);
        bool IsAccountPasswordCorrect(string accountNumber, string password);
    }
}
