using SevColApp.Context;
using SevColApp.Helpers;
using SevColApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SevColApp.Repositories
{
    public class GamemasterRepository : IGamemasterRepository
    {
        private readonly SevColContext _context;
        public GamemasterRepository(SevColContext context)
        {
            _context = context;
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

        private User GetUserByUsername(string userName)
        {
            return _context.Users.Where(x => x.LoginName == userName).FirstOrDefault();
        }
    }
}
