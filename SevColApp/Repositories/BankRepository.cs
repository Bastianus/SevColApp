using Microsoft.EntityFrameworkCore;
using SevColApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace SevColApp.Repositories
{
    public class BankRepository : IBankRepository
    {
        private SevColContext _context;
        public BankRepository(SevColContext context)
        {
            _context = context;
        }
        public async Task<List<BankAccount>> GetBankAccountsOfUser(int userId)
        {
            var accounts = await _context.BankAccounts.Where(x => x.userId == userId).ToListAsync();

            foreach (var account in accounts)
            {
                var bank = await _context.Banks.Where(bank => bank.Id == account.BankId).FirstOrDefaultAsync();

                account.Bank = bank;
            }

            return accounts;
        }

        public async Task<BankAccount> GetBankAccountById(int id)
        {
            return await _context.BankAccounts.FindAsync(id);
        }

        public async Task<List<Bank>> GetAllBanks()
        {
            var allBanks = await _context.Banks.ToListAsync();

            return allBanks.OrderBy(x => x.Name).ToList();
        }

        public void CreateNewAccount(InputOutputAccountCreate input, int userId)
        {
            var bank = GetBankByBankName(input.BankName);

            input.UserHasAccount = _context.BankAccounts.Any(x => x.userId == userId);

            var newAccount = new BankAccount();

            newAccount.AccountName = input.AccountName;

            newAccount.AccountNumber = CreateAccountNumber(bank);

            newAccount.BankId = bank.Id;

            newAccount.Credit = DetermineStartingCredit(input.UserHasAccount, input.WealthLevel);

            newAccount.ExpectedIncome = DetermineExpectedIncome(input.UserHasAccount, input.WealthLevel);

            newAccount.PasswordHash = GetPasswordHash(input.Password);

            newAccount.userId = userId;

            _context.BankAccounts.Add(newAccount);

            _context.SaveChanges();
        }

        public Transfer ExecuteTransfer(Transfer transfer)
        {
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    RemoveAmountFromPayer(transfer);
                    AddAmountToReceiver(transfer);

                    _context.SaveChanges();

                    scope.Complete();
                }
            }            
            catch (TransactionAbortedException ex)
            {
                transfer.Error = ex.Message;
            }

            transfer.Time = DateTime.Now;
            return transfer;
        }

        public async Task<bool> IsAccountPasswordCorrect(string accountNumber, string password)
        {
            var account = await _context.BankAccounts.Where(x => x.AccountNumber == accountNumber).FirstOrDefaultAsync();

            var passwordHash = GetPasswordHash(password);

            return account.PasswordHash.SequenceEqual(passwordHash);
        }

        private byte[] GetPasswordHash(string password)
        {
            if (string.IsNullOrEmpty(password)) return new byte[] { };

            using (HashAlgorithm algorithm = SHA512.Create())
            {
                return algorithm.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }

        private int DetermineStartingCredit(bool userHasAccount, string wealthLevel)
        {
            if (userHasAccount) return 0;

            switch (wealthLevel)
            {
                case "Civilian":
                    return 1250;
                case "Wealthy":
                    return 1750;
                case "Very wealthy":
                    return 28654134;
                default:
                    return 1000;
            }
        }

        private int DetermineExpectedIncome(bool userHasAccount, string wealthLevel)
        {
            if (userHasAccount) return 0;

            switch (wealthLevel)
            {
                case "Civilian":
                    return 750;
                case "Wealthy":
                    return 1250;
                case "Very wealthy":
                    return 1000000;
                default:
                    return 500;
            }
        }

        private string CreateAccountNumber(Bank bank)
        {
            var generator = new Random();

            string generatedAccountNumber;

            do
            {
                generatedAccountNumber = CreateAbbreviationLettersForBankColony(bank);

                generatedAccountNumber += GenerateRandomNumber(generator, 1, 99).ToString();

                generatedAccountNumber += bank.Abbreviation;

                generatedAccountNumber += GenerateRandomNumber(generator, 1, 999999);
            }
            while (_context.BankAccounts.Any(x => x.AccountNumber == generatedAccountNumber));            

            return generatedAccountNumber;
        }

        private int GenerateRandomNumber(Random generator, int min, int max)
        {
            System.Threading.Thread.Sleep(2);
            var randomNumber = generator.Next(min, max);
            System.Threading.Thread.Sleep(3);
            return randomNumber;
        }

        private Bank GetBankByBankName(string bankName)
        {
            var bank = _context.Banks.Where(x => x.Name == bankName).FirstOrDefault();

            return bank;
        }

        private string CreateAbbreviationLettersForBankColony(Bank bank)
        {
            var colony = _context.Colonies.Where(x => x.Id == bank.ColonyId).FirstOrDefault();

            switch (colony.Name)
            {
                case "Earth":
                    return "EA";
                case "Luna":
                    return "LN";
                case "Jupiter":
                    return "JU";
                case "Saturn":
                    return "ST";
                case "Eden and Kordoss":
                    return "EK";
                case "The Worlds of Light":
                    return "WL";
                default:
                    return "MA";
            }
        }

        private List<string> GetAllExistingBankAccountNumbers()
        {
            var allExistingBankAccounts = _context.BankAccounts.ToList();

            var allBankAccountNumbers = new List<string>();

            allExistingBankAccounts.ForEach(account => allBankAccountNumbers.Add(account.AccountNumber));

            return allBankAccountNumbers;
        }

        private void RemoveAmountFromPayer(Transfer transfer)
        {
            var payingAccount = _context.BankAccounts.Where(x => x.AccountNumber == transfer.PayingAccountNumber).FirstOrDefault() ?? throw new TransactionAbortedException("No such account exists");

            if (transfer.Amount > payingAccount.Credit) throw new TransactionAbortedException("Not enough money on paying account");

            payingAccount.Credit -= transfer.Amount;
        }

        private void AddAmountToReceiver(Transfer transfer)
        {
            var receivingAccount = _context.BankAccounts.Where(x => x.AccountNumber == transfer.ReceivingAccountNumber).FirstOrDefault() ?? throw new TransactionAbortedException("No such account exists");

            receivingAccount.Credit += transfer.Amount;
        }
    }
}
