using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SevColApp.Context;
using SevColApp.Helpers;
using SevColApp.Models;
using SevColApp.Repositories;
using Shouldly;
using System;
using System.Linq;

namespace SevColAppTests.RepositoryTests
{
    [TestClass]
    public class BankRepositoryTests
    {
        private SevColContext _context;

        [TestInitialize]
        public void Init()
        {
            var options = new DbContextOptionsBuilder<SevColContext>()
            .UseInMemoryDatabase(databaseName: "SevCol")
            .Options;

            _context = new SevColContext(options);
        }

        [TestCleanup]
        public void CleanUp()
        {
            _context.Database.EnsureDeleted();
        }

        [TestMethod]
        public void GetBankAccountById_ReturnsCorrectBankAccount()
        {
            //arrange
            var firstBankAccount = new BankAccount { Id = 12, userId = 37, BankId = 13, AccountName = "first" };

            var secondBankAccount = new BankAccount { Id = 16, userId = 37, BankId = 17, AccountName = "second" };

            _context.BankAccounts.Add(firstBankAccount);
            _context.BankAccounts.Add(secondBankAccount);

            _context.SaveChanges();

            var sut = new BankRepository(_context);

            //act
            var answer = sut.GetBankAccountById(16);

            //assert
            answer.ShouldNotBeNull();
            answer.AccountName.ShouldBe("second");
            answer.userId.ShouldBe(37);
            answer.BankId.ShouldBe(17);
        }

        [TestMethod]
        public void GetBankAccountsOfUser_ReturnsBankAccountsOfRightUser()
        {
            //arrange    
            var firstBankAccount = new BankAccount { Id = 12, userId = 37, BankId = 13, AccountName = "first" };

            var secondBankAccount = new BankAccount { Id = 16, userId = 37, BankId = 17, AccountName = "second" };

            var wrongBankAccount = new BankAccount { Id = 19, userId = 73, BankId = 17 };

            var firstBank = new Bank(13, "First Bank", "FB", 1);

            var secondBank = new Bank(17, "Second Bank", "SB", 2);

            _context.Banks.Add(firstBank);
            _context.Banks.Add(secondBank);

            _context.BankAccounts.Add(firstBankAccount);
            _context.BankAccounts.Add(secondBankAccount);
            _context.BankAccounts.Add(wrongBankAccount);

            _context.SaveChanges();

            var sut = new BankRepository(_context);

            //act
            var answer = sut.GetBankAccountsOfUser(37);

            //assert
            answer.Count.ShouldBe(2);

            var answerFirst = answer.Where(a => a.AccountName == "first").SingleOrDefault();
            answerFirst.ShouldNotBeNull();
            answerFirst.Bank.ShouldNotBeNull();
            answerFirst.Bank.Name.ShouldBe("First Bank");
            answerFirst.Bank.Abbreviation.ShouldBe("FB");

            var answerSecond = answer.Where(a => a.AccountName == "second").SingleOrDefault();
            answerSecond.ShouldNotBeNull();
            answerSecond.Bank.ShouldNotBeNull();
            answerSecond.Bank.Name.ShouldBe("Second Bank");
            answerSecond.Bank.Abbreviation.ShouldBe("SB");
        }

        [TestMethod]
        public void GetTransfersByAccountNumber_ReturnsTransfersWhereItsThePayer_And_TheAccountsWhereItsTheReceiver_and_InvertsTheAmountWhereItsThePayingAccount()
        {
            //arrange
            var payingAccount = new BankAccount { AccountNumber = "test account number" };
            var dummyAccount = new BankAccount { AccountNumber = "dummy" };
            var wrongAccount = new BankAccount { AccountNumber = "wrong" };

            _context.BankAccounts.Add(payingAccount);
            _context.BankAccounts.Add(dummyAccount);
            _context.BankAccounts.Add(wrongAccount);


            var transfer1 = new Transfer { PayingAccountNumber = "test account number", ReceivingAccountNumber = "dummy", Amount = 58, Time = new DateTime(2019, 12, 8) };
            var transfer2 = new Transfer { PayingAccountNumber = "dummy", ReceivingAccountNumber = "test account number", Amount = 12, Time = new DateTime(2020, 6, 9) };
            var transfer3 = new Transfer { PayingAccountNumber = "dummy", ReceivingAccountNumber = "wrong" };

            _context.Transfers.Add(transfer1);
            _context.Transfers.Add(transfer2);
            _context.Transfers.Add(transfer3);

            _context.SaveChanges();

            var sut = new BankRepository(_context);

            //act
            var answer = sut.GetTransfersByAccountNumber("test account number");

            //assert
            answer.ShouldNotBeNull();

            answer.Count.ShouldBe(2);

            answer[0].Amount.ShouldBe(12);

            answer[1].Amount.ShouldBe(-58);
        }

        [TestMethod]
        public void GetBankAccountDetailsByAccountName_ReturnsTheRightBankAccountDetails()
        {
            //arrange
            var testAccountName = "test account name";
            var rightAccount = new BankAccount { Id = 36, AccountName = testAccountName , AccountNumber = "test account number", Credit = 578};
            var dummyAccount = new BankAccount { AccountNumber = "dummy", AccountName = "dummy" };
            var wrongAccount = new BankAccount { AccountName = "wrong" };

            _context.BankAccounts.Add(rightAccount);
            _context.BankAccounts.Add(dummyAccount);
            _context.BankAccounts.Add(wrongAccount);


            var transfer1 = new Transfer { PayingAccountNumber = "test account number", ReceivingAccountNumber = "dummy", Amount = 58, Time = new DateTime(2019, 12, 8) };
            var transfer2 = new Transfer { PayingAccountNumber = "dummy", ReceivingAccountNumber = "test account number", Amount = 12, Time = new DateTime(2020, 6, 9) };
            var transfer3 = new Transfer { PayingAccountNumber = "dummy", ReceivingAccountNumber = "wrong" };

            _context.Transfers.Add(transfer1);
            _context.Transfers.Add(transfer2);
            _context.Transfers.Add(transfer3);

            _context.SaveChanges();

            var sut = new BankRepository(_context);

            //act
            var answer = sut.GetBankAccountDetailsByAccountName(testAccountName);

            //assert
            answer.ShouldNotBeNull();
            answer.Id.ShouldBe(36);
            answer.AccountName.ShouldBe("test account name");
            answer.AccountNumber.ShouldBe("test account number");
            answer.Credit.ShouldBe(578);
            answer.Transfers.Count.ShouldBe(2);
        }

        [TestMethod]
        public void GetAllBanks_ReturnsAllBanks_ApartFromTheForbiddenBanks()
        {
            //arrange
            var bank1 = new Bank(1, "test bank one", "TB1", 1);
            var bank2 = new Bank(6, "test bank two", "TB2", 6);
            var forbiddenBank = new Bank(8, "SevCol Bank", "SCB", 8);
            var bank3 = new Bank(12, "last test bank", "LTB", 49);

            _context.Banks.Add(bank1);
            _context.Banks.Add(bank2);
            _context.Banks.Add(forbiddenBank);
            _context.Banks.Add(bank3);

            _context.SaveChanges();

            var sut = new BankRepository(_context);

            //act
            var answer = sut.GetAllBanks();

            //assert
            answer.Count.ShouldBe(3);
            answer.Find(b => b.Id == 1 && b.Name == "test bank one" && b.Abbreviation == "TB1" && b.ColonyId == 1).ShouldNotBeNull();
            answer.Find(b => b.Id == 6 && b.Name == "test bank two" && b.Abbreviation == "TB2" && b.ColonyId == 6).ShouldNotBeNull();
            answer.Find(b => b.Id == 12 && b.Name == "last test bank" && b.Abbreviation == "LTB" && b.ColonyId == 49).ShouldNotBeNull();
        }

        [TestMethod]
        public void GetAllBankNames_ReturnsAllBankNames()
        {
            //arrange
            var bank1 = new Bank(1, "test bank one", "TB1", 1);
            var bank2 = new Bank(6, "test bank two", "TB2", 6);
            var bank3 = new Bank(8, "SevCol Bank", "SCB", 8);
            var bank4 = new Bank(12, "last test bank", "LTB", 49);

            _context.Banks.Add(bank1);
            _context.Banks.Add(bank2);
            _context.Banks.Add(bank3);
            _context.Banks.Add(bank4);

            _context.SaveChanges();

            var sut = new BankRepository(_context);

            //act
            var answer = sut.GetAllBankNames();

            //assert
            answer.Count.ShouldBe(4);
            answer.Contains("test bank one").ShouldBeTrue();
            answer.Contains("test bank two").ShouldBeTrue();
            answer.Contains("SevCol Bank").ShouldBeTrue();
            answer.Contains("last test bank").ShouldBeTrue();
        }

        [TestMethod]
        public void GetAllBankAccountNumbers_ReturnsAllBankAccountNumbers()
        {
            //arrange
            var account1 = new BankAccount { AccountNumber = "first" };
            var account2 = new BankAccount { AccountNumber = "second" };
            var account3 = new BankAccount { AccountNumber = "third" };

            _context.BankAccounts.Add(account1);
            _context.BankAccounts.Add(account2);
            _context.BankAccounts.Add(account3);

            _context.SaveChanges();

            var sut = new BankRepository(_context);

            //act
            var answer = sut.GetAllBankAccountNumbers();

            //assert
            answer.Count.ShouldBe(3);
            answer.Contains("first").ShouldBeTrue();
            answer.Contains("second").ShouldBeTrue();
            answer.Contains("third").ShouldBeTrue();
        }

        [TestMethod]
        public void CreateNewAccount_AddsANewAccount()
        {
            //arrange
            var colony = new Colony(5,"Eden and Kordoss");

            _context.Colonies.Add(colony);

            var user = new User { Id = 5 };

            _context.Users.Add(user);

            var bank = new Bank(4, "the test bank", "TTB", 5);

            _context.Banks.Add(bank);

            _context.SaveChanges();

            var sut = new BankRepository(_context);

            var input = new InputOutputAccountCreate
            {
                BankName = "the test bank",

                AccountName = "desired account name",

                Password = "supersafe password"
            };

            //act
            var answer = sut.CreateNewAccount(input, 5);

            //assert
            answer.ShouldNotBeNull();

            answer.AccountName.ShouldBe("desired account name");

            answer.AccountNumber.Length.ShouldBe(13);
            answer.AccountNumber.Substring(0, 2).ShouldBe("EK");
            answer.AccountNumber.Substring(4, 3).ShouldBe("TTB");

            answer.BankId.ShouldBe(4);

            answer.Credit.ShouldBe(1000);

            answer.ExpectedIncome.ShouldBe(500);

            answer.PasswordHash.ShouldBe(PasswordHelper.GetPasswordHash("supersafe password"));

            answer.userId.ShouldBe(5);
        }

        [TestMethod]
        public void CreateNewAccount_WithUnknownBankName_ThrowsNoException_AndCreatesABankAccountWithAnAccountNumber()
        {
            //arrange
            var colony = new Colony(5, "Eden and Kordoss");

            _context.Colonies.Add(colony);

            var user = new User { Id = 5 };

            _context.Users.Add(user);

            var bank = new Bank(4, "the test bank", "TTB", 5);

            _context.Banks.Add(bank);

            _context.SaveChanges();

            var sut = new BankRepository(_context);

            var input = new InputOutputAccountCreate
            {
                BankName = "the test bank with a spelling error",

                AccountName = "desired account name",

                Password = "supersafe password"
            };


            //act
            var answer = sut.CreateNewAccount(input, 5);


            //assert
            answer.ShouldNotBeNull();
            answer.BankId.ShouldNotBe(0);
            answer.AccountNumber.Length.ShouldBe(13);
        }

        [TestMethod]
        public void ExecuteTransfer_HappyFlow()
        {
            //arrange
            var payerBankAccount = new BankAccount { Id = 2, AccountNumber = "payer account number", Credit = 500 };
            var receivingBankAccount = new BankAccount { Id = 8, AccountNumber = "receiver account number", Credit = 20 };

            _context.BankAccounts.Add(payerBankAccount);
            _context.BankAccounts.Add(receivingBankAccount);

            _context.SaveChanges();

            var sut = new BankRepository(_context);

            var input = new Transfer
            {
                Amount = 300,
                PayingAccountNumber = "payer account number",
                ReceivingAccountNumber = "receiver account number",
                Description = "description of the transfer"
            };

            //act
            var answer = sut.ExecuteTransfer(input);

            //assert
            answer.Errors.Count.ShouldBe(0);
            answer.Amount.ShouldBe(300);
            answer.Time.ShouldNotBe(DateTime.MinValue);
            answer.Description.ShouldBe("description of the transfer");

            _context.BankAccounts.Find(2).Credit.ShouldBe(200);
            _context.BankAccounts.Find(8).Credit.ShouldBe(320);

            _context.Transfers.Where(t => t.Description == "description of the transfer").FirstOrDefault().ShouldNotBeNull();
        }

        [TestMethod]
        public void ExecuteTransfer_WhenPayerBankAccountUnknown_GivesCorrectErrorAndNoChangeToCredit()
        {
            //arrange
            var payerBankAccount = new BankAccount { Id = 2, AccountNumber = "payer account number", Credit = 500 };
            var receivingBankAccount = new BankAccount { Id = 8, AccountNumber = "receiver account number", Credit = 20 };

            _context.BankAccounts.Add(payerBankAccount);
            _context.BankAccounts.Add(receivingBankAccount);

            _context.SaveChanges();

            var sut = new BankRepository(_context);

            var input = new Transfer
            {
                Amount = 300,
                PayingAccountNumber = "not the payer account number",
                ReceivingAccountNumber = "receiver account number",
                Description = "description of the transfer"
            };

            //act
            var answer = sut.ExecuteTransfer(input);

            //assert
            answer.Errors.Count.ShouldBe(1);
            answer.Errors.First().ShouldBe("No such account exists");

            _context.BankAccounts.Find(2).Credit.ShouldBe(500);
            _context.BankAccounts.Find(8).Credit.ShouldBe(20);

            _context.Transfers.Where(t => t.Description == "description of the transfer").FirstOrDefault().ShouldBeNull();
        }

        [TestMethod]
        public void ExecuteTransfer_WhenReceiverBankAccountUnknown_GivesCorrectErrorAndNoChangeToCredit()
        {
            //arrange
            var payerBankAccount = new BankAccount { Id = 2, AccountNumber = "payer account number", Credit = 500 };
            var receivingBankAccount = new BankAccount { Id = 8, AccountNumber = "receiver account number", Credit = 20 };

            _context.BankAccounts.Add(payerBankAccount);
            _context.BankAccounts.Add(receivingBankAccount);

            _context.SaveChanges();

            var sut = new BankRepository(_context);

            var input = new Transfer
            {
                Amount = 300,
                PayingAccountNumber = "payer account number",
                ReceivingAccountNumber = "not the receiver account number",
                Description = "description of the transfer"
            };

            //act
            var answer = sut.ExecuteTransfer(input);

            //assert
            answer.Errors.Count.ShouldBe(1);
            answer.Errors.First().ShouldBe("No such account exists");

            _context.BankAccounts.Find(2).Credit.ShouldBe(500);
            _context.BankAccounts.Find(8).Credit.ShouldBe(20);

            _context.Transfers.Where(t => t.Description == "description of the transfer").FirstOrDefault().ShouldBeNull();
        }

        [TestMethod]
        public void ExecuteTransfer_WHenPayerHasInsufficientMoney_GivesCorrectAnswerAndNoChangeToCredit()
        {
            //arrange
            var payerBankAccount = new BankAccount { Id = 2, AccountNumber = "payer account number", Credit = 200 };
            var receivingBankAccount = new BankAccount { Id = 8, AccountNumber = "receiver account number", Credit = 20 };

            _context.BankAccounts.Add(payerBankAccount);
            _context.BankAccounts.Add(receivingBankAccount);

            _context.SaveChanges();

            var sut = new BankRepository(_context);

            var input = new Transfer
            {
                Amount = 300,
                PayingAccountNumber = "payer account number",
                ReceivingAccountNumber = "receiver account number",
                Description = "description of the transfer"
            };

            //act
            var answer = sut.ExecuteTransfer(input);

            //assert
            answer.Errors.Count.ShouldBe(1);
            answer.Errors.First().ShouldBe("Not enough money on paying account");

            _context.BankAccounts.Find(2).Credit.ShouldBe(200);
            _context.BankAccounts.Find(8).Credit.ShouldBe(20);

            _context.Transfers.Where(t => t.Description == "description of the transfer").FirstOrDefault().ShouldBeNull();
        }

        [TestMethod]
        public void IsAccountPasswordCorrect_HappyFlow()
        {
            //arrange
            var password = "this is a password";

            var passwordHash = PasswordHelper.GetPasswordHash(password);

            var account = new BankAccount { AccountNumber = "this is an account number", PasswordHash = passwordHash };

            _context.BankAccounts.Add(account);

            _context.SaveChanges();

            var sut = new BankRepository(_context);

            //act
            var answer = sut.IsAccountPasswordCorrect("this is an account number", "this is a password");

            //assert
            answer.ShouldBeTrue();
        }

        [TestMethod]
        public void IsAccountPasswordCorrect_WrongPasswordReturnsFalse()
        {
            //arrange
            var password = "this is a password";

            var passwordHash = PasswordHelper.GetPasswordHash(password);

            var account = new BankAccount { AccountNumber = "this is an account number", PasswordHash = passwordHash };

            _context.BankAccounts.Add(account);

            _context.SaveChanges();

            var sut = new BankRepository(_context);

            //act
            var answer = sut.IsAccountPasswordCorrect("this is an accountnumber", "this is the wrong password");

            //assert
            answer.ShouldBeFalse();
        }

        [TestMethod]
        public void IsAccountPasswordCorrect_WhenAccountNumberIsUnknown_ReturnsFalse()
        {
            //arrange
            var password = "this is a password";

            var passwordHash = PasswordHelper.GetPasswordHash(password);

            var account = new BankAccount { AccountNumber = "this is an account number", PasswordHash = passwordHash };

            _context.BankAccounts.Add(account);

            _context.SaveChanges();

            var sut = new BankRepository(_context);

            //act
            var answer = sut.IsAccountPasswordCorrect("this is the wrong accountnumber", "this is a password");

            //assert
            answer.ShouldBeFalse();
        }

    }
}
