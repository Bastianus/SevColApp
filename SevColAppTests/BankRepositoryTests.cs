using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SevColApp.Context;
using SevColApp.Models;
using SevColApp.Repositories;
using Shouldly;
using System;
using System.Linq;

namespace SevColAppTests
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
            answer.Transfers.Count().ShouldBe(2);
        }
    }
}
