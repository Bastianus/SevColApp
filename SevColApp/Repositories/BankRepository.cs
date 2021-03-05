using Microsoft.EntityFrameworkCore;
using SevColApp.Context;
using SevColApp.Helpers;
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
        private readonly SevColContext _context;
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

            var newAccount = new BankAccount
            {
                AccountName = input.AccountName,

                AccountNumber = CreateAccountNumber(bank),

                BankId = bank.Id,

                Credit = CreditHelper.DetermineStartingCredit(input.UserHasAccount, input.WealthLevel),

                ExpectedIncome = CreditHelper.DetermineExpectedIncome(input.UserHasAccount, input.WealthLevel),

                PasswordHash = PasswordHelper.GetPasswordHash(input.Password),

                userId = userId
            };

            _context.BankAccounts.Add(newAccount);

            _context.SaveChanges();
        }

        public Transfer ExecuteTransfer(Transfer transfer)
        {
            try
            {
                using TransactionScope scope = new TransactionScope();
                RemoveAmountFromPayer(transfer);
                AddAmountToReceiver(transfer);

                _context.SaveChanges();

                scope.Complete();
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

            return PasswordHelper.PasswordCheck(password, account.PasswordHash);
        }        

        private string CreateAccountNumber(Bank bank)
        {
            var generator = new Random();

            string generatedAccountNumber;

            do
            {
                generatedAccountNumber = CreateAbbreviationLettersForBankColony(bank);

                generatedAccountNumber += CreditHelper.GenerateRandomNumber(generator, 1, 99).ToString();

                generatedAccountNumber += bank.Abbreviation;

                generatedAccountNumber += CreditHelper.GenerateRandomNumber(generator, 1, 999999);
            }
            while (_context.BankAccounts.Any(x => x.AccountNumber == generatedAccountNumber));            

            return generatedAccountNumber;
        }        

        private Bank GetBankByBankName(string bankName)
        {
            var bank = _context.Banks.Where(x => x.Name == bankName).FirstOrDefault();

            return bank;
        }

        private string CreateAbbreviationLettersForBankColony(Bank bank)
        {
            var colony = _context.Colonies.Where(x => x.Id == bank.ColonyId).FirstOrDefault();

            return colony.Name switch
            {
                "Earth" => "EA",
                "Luna" => "LN",
                "Jupiter" => "JU",
                "Saturn" => "ST",
                "Eden and Kordoss" => "EK",
                "The Worlds of Light" => "WL",
                _ => "MA",
            };
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
