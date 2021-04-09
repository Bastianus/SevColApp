using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SevColApp.Context;
using SevColApp.Hosted_service;
using SevColApp.Models;
using SevColApp.Repositories;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SevColAppTests.StockTests
{
    [TestClass]
    public class StocksExchangerTests
    {
        private SevColContext _context;
        private Mock<ILogger<SevColContext>> _logger;

        [TestInitialize]
        public void Init()
        {
            var options = new DbContextOptionsBuilder<SevColContext>()
            .UseInMemoryDatabase(databaseName: "SevCol")
            .Options;

            _context = new SevColContext(options);

            _logger = new Mock<ILogger<SevColContext>>();

        }

        [TestCleanup]
        public void CleanUp()
        {
            _context.Database.EnsureDeleted();
        }

        [TestMethod]
        public void ExchangeStocksForCompany_OneBuyer_OneSeller_OfferMoreThanMinimum_CreatesOneTransfer()
        {
            //arrange
            uint numberOfStocksRequested = 3;
            uint numberOfStocksOffered = 3;

            uint numberOfStocksSellerOwns = 80;

            uint offerCredits = 100;
            uint minimumCredits = 0;

            int sellerId = 9;
            int buyerId = 6;

            int sellerStartingCredit = 0;
            int buyerStartingCredit = 500;

            int companyId = 7;

            var sellerBankAccount = new BankAccount { userId = sellerId, Credit = sellerStartingCredit, AccountNumber = "sellerAN" };

            var buyerBankAccount = new BankAccount { userId = buyerId, Credit = buyerStartingCredit, AccountNumber = "buyerAN" };

            _context.BankAccounts.Add(sellerBankAccount);
            _context.BankAccounts.Add(buyerBankAccount);

            var sellerStocks = new UserCompanyStocks { userId = sellerId, companyId = companyId, NumberOfStocks = numberOfStocksSellerOwns };

            _context.UserCompanyStocks.Add(sellerStocks);

            var time = new DateTime(2021, 3, 21, 15, 46, 37);

            var company = new Company { Id = companyId, Name = "a company"};

            var buyRequests = new List<StockExchangeBuyRequest> { new StockExchangeBuyRequest 
            { 
                OfferPerStock = offerCredits,
                NumberOfStocks = numberOfStocksRequested,
                userId = buyerId,
                companyId = companyId,
                AccountNumber = "buyerAN"
            } };

            var sellRequests = new List<StockExchangeSellRequest> { new StockExchangeSellRequest
            {
                MinimumPerStock = minimumCredits,
                NumberOfStocks = numberOfStocksOffered,
                userId = sellerId,
                companyId = companyId,
                AccountNumber = "sellerAN"
            } };


            _context.StockExchangeBuyRequests.AddRange(buyRequests);
            _context.StockExchangeSellRequests.AddRange(sellRequests);

            _context.SaveChanges();

            var repo = new StocksRepository(_context);

            var bankRepo = new BankRepository(_context);

            var sut = new StocksExchanger(_logger.Object, repo, bankRepo);

            //act
            sut.ExchangeStocksForCompany(company, time);

            //assert
            _context.StockExchangesCompleted.ToList().Count.ShouldBe(1);

            var completedExchange = _context.StockExchangesCompleted.First();
            ((int)completedExchange.NumberOfStocks).ShouldBe(3);
            completedExchange.sellerId.ShouldBe(sellerId);
            completedExchange.buyerId.ShouldBe(buyerId);
            completedExchange.ExchangeDateAndTime.ShouldBe(time);
            completedExchange.companyId.ShouldBe(companyId);
            ((int)completedExchange.AmountPerStock).ShouldBe((int)offerCredits);

            _context.BankAccounts.Where(ba => ba.userId == buyerId).Single().Credit.ShouldBe(buyerStartingCredit - completedExchange.AmountPerStock * completedExchange.NumberOfStocks);

            _context.BankAccounts.Where(ba => ba.userId == sellerId).Single().Credit.ShouldBe(sellerStartingCredit + completedExchange.AmountPerStock * completedExchange.NumberOfStocks);
        }
    }
}
