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
        private readonly IBankRepository _bankRepo;
        public GamemasterRepository(SevColContext context, CookieHelper cookieHelper, IBankRepository bankRepo)
        {
            _context = context;
            _cookieHelper = cookieHelper;
            _bankRepo = bankRepo;
        }

        public List<User> GetAllUsers()
        {
            return  _context.Users.Where(x => x.Id != _cookieHelper.GetGameMasterId()).ToList();
        }

        public UserAccountsAnswer GetAllAccountsOfUser(string userName)
        {
            var answer = new UserAccountsAnswer
            {
                User = GetUserByUsername(userName)
            };

            if (answer.User == null)
            {
                answer.Error = $"No user with login name \"{userName}\" was found.";

                return answer;
            }

            answer.Accounts = _context.BankAccounts.Where(x => x.userId == answer.User.Id).ToList();

            if (answer.Accounts.Count == 0)
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
        public BankAccount GetAccountByAccountNumber(string accountNumber)
        {
            return _context.BankAccounts.Where(x => x.AccountNumber == accountNumber).FirstOrDefault();
        }

        public EditBankAccountResult EditBankAccount(InputOutputAccountEdit input)
        {
            var answer = new EditBankAccountResult { Errors = new List<string>()};

            var toAccount = GetAccountByAccountNumber(input.ToAccountNumber);

            if (toAccount == null) 
            { 
                input.Errors.Add("Account to edit not found."); 
            }
            else if (input.FromRandomSevColAccount == true && !string.IsNullOrEmpty(input.AccountNumber))
            {
                answer.Errors.Add("You can't have both a random SevCol account and a chosen account number.");
            }
            else
            {
                if (!string.IsNullOrEmpty(input.AccountNumber))
                {
                    var fromAccount = GetAccountByAccountNumber(input.AccountNumber);

                    if (fromAccount == null)
                    {
                        answer.Errors.Add($"Account number \"{input.AccountNumber}\" was not found.");
                    }
                    else
                    {
                        answer = TransferBetweenAccounts(fromAccount, toAccount, input.Amount, input.Description, answer);  
                    }
                }
                else if (input.FromRandomSevColAccount)
                {
                    var randomAccountName = Guid.NewGuid().ToString().Substring(0, 10);

                    var randomPassword = Guid.NewGuid().ToString().Substring(0, 10);

                    var newAccountInput = new InputOutputAccountCreate
                    {
                        AccountName = randomAccountName,
                        BankName = "SevCol Bank",
                        Password = randomPassword,
                        UserHasAccount = true,
                        WealthLevel = "Very wealthy"
                    };

                    var gmId = _cookieHelper.GetGameMasterId();

                    var newSevColAccount = _bankRepo.CreateNewAccount(newAccountInput, gmId);

                    answer = TransferBetweenAccounts(newSevColAccount, toAccount, input.Amount, input.Description, answer);
                }
                else
                {
                    toAccount.Credit += input.Amount;

                    if(input.Amount >= 0)
                    {
                        answer.FromAccount = "\"no account\"";
                        answer.ToAccount = toAccount.AccountNumber;
                    }
                    else
                    {
                        answer.FromAccount = toAccount.AccountNumber;
                        answer.ToAccount = "\"no account\"";
                    }

                    answer.AmountChange = input.Amount;
                }
            }
            
            _context.SaveChanges();

            return answer;
        }

        private User GetUserByUsername(string userName)
        {
            return _context.Users.Where(x => x.LoginName == userName).FirstOrDefault();
        }

        private EditBankAccountResult TransferBetweenAccounts(BankAccount fromAccount, BankAccount toAccount, int amount, string description, EditBankAccountResult answer)
        {
            var transfer = new Transfer
            {
                Amount = Math.Abs(amount),
                Description = description,
                Time = DateTime.Now
            };

            if (amount >= 0)
            {
                transfer.PayingAccountNumber = fromAccount.AccountNumber;
                transfer.ReceivingAccountNumber = toAccount.AccountNumber;
                
                answer.FromAccount = fromAccount.AccountNumber;
                answer.ToAccount = toAccount.AccountNumber;
            }
            else
            {
                transfer.PayingAccountNumber = toAccount.AccountNumber;
                transfer.ReceivingAccountNumber = fromAccount.AccountNumber;
               
                answer.FromAccount = toAccount.AccountNumber;
                answer.ToAccount = fromAccount.AccountNumber;
            }

            fromAccount.Credit -= amount;
            toAccount.Credit += amount;

            answer.NewAmount = toAccount.Credit;
            answer.AmountChange = Math.Abs(amount);

            _context.Transfers.Add(transfer);

            _context.SaveChanges();

            return answer;
        }
    }
}
