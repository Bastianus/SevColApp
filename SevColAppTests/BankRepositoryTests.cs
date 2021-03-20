using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SevColApp.Context;
using SevColApp.Models;
using SevColApp.Repositories;
using Shouldly;
using System.Linq;

namespace SevColAppTests
{
    [TestClass]
    public class BankRepositoryTests
    {
        private SevColContext context;

        [TestInitialize]
        public void Init()
        {
            var options = new DbContextOptionsBuilder<SevColContext>()
            .UseInMemoryDatabase(databaseName: "SevCol")
            .Options;

            context = new SevColContext(options);
        }

        [TestMethod]
        public void GetBankAccountsOfUser_ReturnsBankAccountsOfRightUser()
        {
            //arrange
            var firstBankAccount = new BankAccount { userId = 37,  BankId = 13, AccountName = "first"};

            var secondBankAccount = new BankAccount { userId = 37, BankId = 17, AccountName = "second"};

            var wrongBankAccount = new BankAccount { userId = 73, BankId = 17 };

            var firstBank = new Bank(13, "First Bank", "FB", 1);

            var secondBank = new Bank(17, "Second Bank", "SB", 2);

            context.Banks.Add(firstBank);
            context.Banks.Add(secondBank);

            context.BankAccounts.Add(firstBankAccount);
            context.BankAccounts.Add(secondBankAccount);
            context.BankAccounts.Add(wrongBankAccount);

            context.SaveChanges();

            var sut = new BankRepository(context);

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

        private void PrepareContextWithBankAccounts()
        {

        }
    }
}
