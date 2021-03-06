﻿using SevColApp.Context;
using SevColApp.Helpers;
using SevColApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;

namespace SevColApp.Repositories
{
    public class BankRepository : IBankRepository
    {
        private readonly SevColContext _context;
        private readonly List<string> _forbiddenBanks;
        public BankRepository(SevColContext context)
        {
            _context = context;
            _forbiddenBanks = new List<string>
            {
                "SevCol Bank"
            };
        }
        public List<BankAccount> GetBankAccountsOfUser(int userId)
        {
            var accounts = _context.BankAccounts.Where(x => x.userId == userId).ToList();

            foreach (var account in accounts)
            {
                var bank = _context.Banks.Where(bank => bank.Id == account.BankId).FirstOrDefault();

                account.Bank = bank;
            }

            return accounts;
        }

        public BankAccount GetBankAccountById(int id)
        {
            return _context.BankAccounts.Find(id);
        }

        public List<Transfer> GetTransfersByAccountNumber(string accountNumber)
        {
            var transfers = _context.Transfers.Where(x => x.PayingAccountNumber == accountNumber).ToList();
            transfers.ForEach(x => x.Amount = -x.Amount);

            transfers.AddRange(_context.Transfers.Where(x => x.ReceivingAccountNumber == accountNumber).ToList());

            return transfers.OrderByDescending(x => x.Time).ToList();
        }

        public BankAccountDetails GetBankAccountDetailsByAccountName(string accountName)
        {
            var data = _context.BankAccounts.Where(x => x.AccountName == accountName).FirstOrDefault();

            var transfers = GetTransfersByAccountNumber(data.AccountNumber);

            return new BankAccountDetails
            {
                Id = data.Id,
                AccountName = data.AccountName,
                AccountNumber = data.AccountNumber,
                Credit = data.Credit,
                Transfers = transfers
            };
        }

        public List<Bank> GetAllBanks()
        {
            var allBanks = _context.Banks.ToList();

            allBanks = FilterForbiddenBanks(allBanks, _forbiddenBanks);

            return allBanks.OrderBy(x => x.Name).ToList();
        }

        public List<string> GetAllBankNames()
        {
            var allBankNames = _context.Banks.Select(x => x.Name).ToList();

            return allBankNames;
        }

        public List<string> GetAllBankAccountNumbers()
        {
            return _context.BankAccounts.Select(x => x.AccountNumber).ToList();
        }

        public BankAccount CreateNewAccount(InputOutputAccountCreate input, int userId)
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

            return newAccount;
        }

        public Transfer ExecuteTransfer(Transfer transfer)
        {
            transfer.Errors = new List<string>();

            try
            {
                CheckBankAccounts(transfer);
                RemoveAmountFromPayer(transfer);
                AddAmountToReceiver(transfer);

                transfer.Time = DateTime.Now;
                _context.Transfers.Add(transfer);

                _context.SaveChanges();

            }
            catch (TransactionAbortedException ex)
            {
                transfer.Errors.Add(ex.Message);
            }


            transfer.Time = DateTime.Now;
            return transfer;
        }

        public BankAccount GetBankAccountByAccountNumber(string accountNumber)
        {
            return _context.BankAccounts.SingleOrDefault(ba => ba.AccountNumber == accountNumber);
        }

        public bool IsAccountPasswordCorrect(string accountNumber, string password)
        {
            var account = _context.BankAccounts.Where(x => x.AccountNumber == accountNumber).FirstOrDefault();

            if (account == null) return false;

            return PasswordHelper.PasswordCheck(password, account.PasswordHash);
        }

        private string CreateAccountNumber(Bank bank)
        {
            var generator = new Random();

            string generatedAccountNumber;

            do
            {
                generatedAccountNumber = CreateAbbreviationLettersForBankColony(bank);

                generatedAccountNumber += CreditHelper.GenerateRandomNumber(generator, 10, 99).ToString();

                generatedAccountNumber += bank.Abbreviation;

                generatedAccountNumber += CreditHelper.GenerateRandomNumber(generator, 100000, 999999);
            }
            while (_context.BankAccounts.Any(x => x.AccountNumber == generatedAccountNumber));

            return generatedAccountNumber;
        }

        private Bank GetBankByBankName(string bankName)
        {
            var bank = _context.Banks.Where(x => x.Name == bankName).FirstOrDefault();

            if (bank == null) // bank name is not known
            {
                bank = _context.Banks.First();
            }

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
                "SevCol" => "SC",
                _ => "MA",
            };
        }

        private void CheckBankAccounts(Transfer transfer)
        {
            if (_context.BankAccounts.Where(x => x.AccountNumber == transfer.PayingAccountNumber).FirstOrDefault() == null) throw new TransactionAbortedException("No such account exists");

            if(_context.BankAccounts.Where(x => x.AccountNumber == transfer.ReceivingAccountNumber).FirstOrDefault() == null) throw new TransactionAbortedException("No such account exists");

            if (transfer.PayingAccountNumber == transfer.ReceivingAccountNumber) throw new TransactionAbortedException($"Receiving account cannot be equal to the paying account.");
        }

        private void RemoveAmountFromPayer(Transfer transfer)
        {
            var payingAccount = _context.BankAccounts.Where(x => x.AccountNumber == transfer.PayingAccountNumber).First();

            if (transfer.Amount > payingAccount.Credit) throw new TransactionAbortedException("Not enough money on paying account");

            payingAccount.Credit -= transfer.Amount;
        }

        private void AddAmountToReceiver(Transfer transfer)
        {
            var receivingAccount = _context.BankAccounts.Where(x => x.AccountNumber == transfer.ReceivingAccountNumber).First();

            receivingAccount.Credit += transfer.Amount;
        }

        private static List<Bank> FilterForbiddenBanks(List<Bank> banks, List<string> forbiddenBanks)
        {
            return banks.Where(bank => !forbiddenBanks.Contains(bank.Name)).ToList();
        }
    }
}
