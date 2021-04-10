using SevColApp.Models;
using System.Collections.Generic;

namespace SevColApp.Repositories
{
    public interface IGamemasterRepository
    {
        List<User> GetAllUsers();
        List<UserWithExtraData> GetAllUsersWithExtraData();
        UserAccountsAnswer GetAllAccountsOfUser(string userName);
        UserPasswordChange ChangeUserPassword(UserPasswordChange input);
        AccountPasswordChange ChangeBankAccountPassword(AccountPasswordChange input);
        List<UserWithExtraData> PayAllowanceForUser(string userLoginName);
        List<UserWithExtraData> ResetAllowances();
        BankAccount GetAccountByAccountNumber(string accountNumber);
        EditBankAccountResult EditBankAccount(InputOutputAccountEdit input);
        Company AddCompany(string companyName);
        AddStocksInputOutput AddStocksForUser(AddStocksInputOutput input);
        User GetUserByLoginName(string loginName);
        List<UserStocks> GetStocksForUserByLoginName(string loginName);
    }
}
