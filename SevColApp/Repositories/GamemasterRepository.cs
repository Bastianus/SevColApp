using SevColApp.Context;
using SevColApp.Helpers;
using SevColApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SevColApp.Repositories
{
    public class GamemasterRepository : IGamemasterRepository
    {
        private readonly SevColContext _context;
        private readonly CookieHelper _cookieHelper;
        public GamemasterRepository(SevColContext context, CookieHelper cookieHelper)
        {
            _context = context;
            _cookieHelper = cookieHelper;
        }

        public List<User> GetAllUsers()
        {
            return  _context.Users.Where(x => x.Id != _cookieHelper.GetGameMasterId()).ToList();
        }

        public UserAccountsAnswer GetAllAccountsOfUser(string userName)
        {
            var answer = new UserAccountsAnswer();

            answer.User = GetUserByUsername(userName);

            if(answer.User == null)
            {
                answer.Error = $"No user with login name \"{userName}\" was found.";

                return answer;
            }

            answer.Accounts = _context.BankAccounts.Where(x => x.userId == answer.User.Id).ToList();

            if (answer.Accounts.Count() == 0)
            {
                answer.Error = $"The user with login name \"{userName}\" has no known bank accounts (yet?).";
            }

            return answer;
        }

        public UserPasswordChange ChangeUserPassword(UserPasswordChange input)
        {
            var user = GetUserByUsername(input.UserLoginName);

            if(user == null)
            {
                input.Error = $"User with username \"{input.UserLoginName}\" not found.";

                return input;
            }

            user.PasswordHash = PasswordHelper.GetPasswordHash(input.NewPassword);

            _context.SaveChanges();

            return input;
        }

        public AccountPasswordChange ChangeBankAccountPassword(AccountPasswordChange input)
        {
            var user = GetUserByUsername(input.UserName);

            if (user == null)
            {
                input.Error = $"User with username \"{input.UserName}\" not found.";

                return input;
            }

            var account = _context.BankAccounts.Where(x => x.userId == user.Id && x.AccountName == input.BankAccountName).FirstOrDefault();

            if (account == null)
            {
                input.Error = $"Bank account with name \"{input.BankAccountName}\" not found.";

                return input;
            }

            account.PasswordHash = PasswordHelper.GetPasswordHash(input.NewPassword);

            _context.SaveChanges();

            return input;
        }

        public AllUsers PayAllowanceForUser(string userLoginName)
        {
            var allUsers = GetAllUsers();

            var rightUser = allUsers.Where(x => x.LoginName == userLoginName).FirstOrDefault();

            var bankAccountsOfUser = _context.BankAccounts.Where(account => account.userId == rightUser.Id).ToList();

            if (rightUser.AllowanceStatus == "Paid" || rightUser.AllowanceStatus == "Already paid")
            {
                rightUser.AllowanceStatus = "Already paid";
            }
            else if(bankAccountsOfUser.Count > 0)
            {
                bankAccountsOfUser.ForEach(account => account.Credit += account.ExpectedIncome);                

                rightUser.AllowanceStatus = "Paid";
            }
            else
            {
                rightUser.AllowanceStatus = "No bank account";
            }

            _context.SaveChanges();

            return new AllUsers { Users = allUsers };
        }

        public AllUsers ResetAllowances()
        {
            var allUsers = GetAllUsers();

            allUsers.ForEach(user => user.AllowanceStatus = "");

            _context.SaveChanges();

            return new AllUsers { Users = allUsers };
        }

        private User GetUserByUsername(string userName)
        {
            return _context.Users.Where(x => x.LoginName == userName).FirstOrDefault();
        }
    }
}
