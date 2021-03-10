using SevColApp.Models;
using System.Collections.Generic;

namespace SevColApp.Repositories
{
    public interface IGamemasterRepository
    {
        List<User> GetAllUsers();
        UserAccountsAnswer GetAllAccountsOfUser(string userName);
        UserPasswordChange ChangeUserPassword(UserPasswordChange input);
        AccountPasswordChange ChangeBankAccountPassword(AccountPasswordChange input);
        AllUsers PayAllowanceForUser(string userLoginName);
        AllUsers ResetAllowances();
        BankAccount GetAccountByAccountNumber(string accountNumber);
    }
}
