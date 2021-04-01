using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SevColApp.Context;
using SevColApp.Hosted_service;
using SevColApp.Models;
using SevColApp.Repositories;
using Shouldly;
using System.Collections.Generic;
using System.Linq;

namespace SevColAppTests.StockTests
{
    [TestClass]
    public class StockInputCheckerTests
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
        public void CheckAllSellRequestsAndRemoveInvalids_WhenOneCorrectSellRequestInContext_DoesNotRemoveTheSellRequest()
        {
            //arrange
            int userId = 7;
            uint userStocks = 80;
            int companyId = 4;

            var company = new Company { Id = companyId };
            _context.Companies.Add(company);

            var userBankAccount = new BankAccount { userId = userId };
            _context.BankAccounts.Add(userBankAccount);

            var userCompanyStocks = new UserCompanyStocks { companyId = companyId, userId = userId, NumberOfStocks = userStocks };
            _context.UserCompanyStocks.Add(userCompanyStocks);

            var sellRequests = new List<StockExchangeSellRequest>
            {
                new StockExchangeSellRequest { userId = userId, companyId = companyId, MinimumPerStock = 35, NumberOfStocks = 6},
            };
            _context.StockExchangeSellRequests.AddRange(sellRequests);

            _context.SaveChanges();

            var repo = new StocksRepository(_context);

            var sut = new StockInputChecker(repo);

            //act
            sut.CheckAllSellRequestsAndRemoveInvalids();

            //assert
            _context.StockExchangeSellRequests.ToList().Count.ShouldBe(1);
        }

        [TestMethod]
        public void CheckAllSellRequestsAndRemoveInvalids_WhenSellerHasNoBankAccount_RemovesTheSellRequest()
        {
            //arrange
            int userId = 7;
            uint userStocks = 80;
            int companyId = 4;

            var company = new Company { Id = companyId };
            _context.Companies.Add(company);
            

            var userCompanyStocks = new UserCompanyStocks { companyId = companyId, userId = userId, NumberOfStocks = userStocks };
            _context.UserCompanyStocks.Add(userCompanyStocks);

            var sellRequests = new List<StockExchangeSellRequest>
            {
                new StockExchangeSellRequest { userId = userId, companyId = companyId, MinimumPerStock = 35, NumberOfStocks = 6},
            };
            _context.StockExchangeSellRequests.AddRange(sellRequests);

            _context.SaveChanges();

            var repo = new StocksRepository(_context);

            var sut = new StockInputChecker(repo);

            //act
            sut.CheckAllSellRequestsAndRemoveInvalids();

            //assert
            _context.StockExchangeSellRequests.ToList().Count.ShouldBe(0);
        }

        [TestMethod]
        public void CheckAllSellRequestsAndRemoveInvalids_WhenUserDoesNotHaveEnoughStocksForAllRequests_KeepsTheSmallestRequestsAndRemovesTheLargestRequests()
        {
            //arrange
            int userId = 7;
            uint userStocks = 80;
            int companyId = 4;

            var company = new Company { Id = companyId };
            _context.Companies.Add(company);

            var userBankAccount = new BankAccount { userId = userId };
            _context.BankAccounts.Add(userBankAccount);

            var userCompanyStocks = new UserCompanyStocks { companyId = companyId, userId = userId, NumberOfStocks = userStocks };
            _context.UserCompanyStocks.Add(userCompanyStocks);

            var sellRequests = new List<StockExchangeSellRequest>
            {
                new StockExchangeSellRequest { userId = userId, companyId = companyId, MinimumPerStock = 35, NumberOfStocks = 6},
                new StockExchangeSellRequest { userId = userId, companyId = companyId, MinimumPerStock = 35, NumberOfStocks = 10},
                new StockExchangeSellRequest { userId = userId, companyId = companyId, MinimumPerStock = 35, NumberOfStocks = 40},
                new StockExchangeSellRequest { userId = userId, companyId = companyId, MinimumPerStock = 35, NumberOfStocks = 50},
                new StockExchangeSellRequest { userId = userId, companyId = companyId, MinimumPerStock = 35, NumberOfStocks = 90},
            };
            _context.StockExchangeSellRequests.AddRange(sellRequests);

            _context.SaveChanges();

            var repo = new StocksRepository(_context);

            var sut = new StockInputChecker(repo);

            //act
            sut.CheckAllSellRequestsAndRemoveInvalids();

            //assert
            _context.StockExchangeSellRequests.ToList().Count.ShouldBe(3);

            var numberOfStocks = _context.StockExchangeSellRequests.Where(sr => sr.userId == userId).Select(sr => (int)sr.NumberOfStocks).ToList();

            numberOfStocks.ShouldContain(6);
            numberOfStocks.ShouldContain(10);
            numberOfStocks.ShouldContain(40);
        }

        [TestMethod]
        public void CheckAllBuyRequestsAndRemoveInvalids_WhenOneCorrectBuyRequestExists_ItIsNotRemoved()
        {
            //arrange
            int userId = 9;
            int companyId = 19;
            int userCredits = 1500;

            var company = new Company { Id = companyId };
            _context.Companies.Add(company);

            var userBankAccount = new BankAccount { userId = userId, Credit = userCredits };
            _context.BankAccounts.Add(userBankAccount);

            var buyRequests = new List<StockExchangeBuyRequest>
            {
                new StockExchangeBuyRequest{ userId = userId, companyId = companyId, OfferPerStock = 300, NumberOfStocks = 4},
            };
            _context.StockExchangeBuyRequests.AddRange(buyRequests);

            _context.SaveChanges();

            var repo = new StocksRepository(_context);

            var sut = new StockInputChecker(repo);

            //act
            sut.CheckAllBuyRequestsAndRemoveInvalids();

            //assert
            _context.StockExchangeBuyRequests.ToList().Count.ShouldBe(1);
        }
    }
}
