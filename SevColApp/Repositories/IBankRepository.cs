using SevColApp.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SevColApp.Repositories
{
    public interface IBankRepository
    {
        List<BankAccount> GetBankAccountsOfUser(int userId);
        BankAccount GetBankAccountById(int id);
        List<Transfer> GetTransfersByAccountNumber(string accountNumber);
        BankAccountDetails GetBankAccountDetailsByAccountName(string accountName);
        List<Bank> GetAllBanks();
        List<string> GetAllBankNames();
        List<string> GetAllBankAccountNumbers();
        BankAccount CreateNewAccount(InputOutputAccountCreate input, int userId);
        Transfer ExecuteTransfer(Transfer transfer);
        bool IsAccountPasswordCorrect(string accountNumber, string password);
    }
}
