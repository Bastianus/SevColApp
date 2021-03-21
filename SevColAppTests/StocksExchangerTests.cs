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

namespace SevColAppTests
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
        public void HappyFlow()
        {
            //arrange
            var sellerBankAccount = new BankAccount { userId = 9 };

            var buyerBankAccount = new BankAccount { userId = 6, Credit = 500 };

            _context.BankAccounts.Add(sellerBankAccount);
            _context.BankAccounts.Add(buyerBankAccount);

            var sellerStocks = new UserCompanyStocks { userId = 9, companyId = 7, NumberOfStocks = 80 };

            _context.UserCompanyStocks.Add(sellerStocks);

            var time = new DateTime(2021, 3, 21, 15, 46, 37);

            var company = new Company { Id = 7, Name = "a company"};

            var buyRequests = new List<StockExchangeBuyRequest> { new StockExchangeBuyRequest 
            { 
                OfferPerStock = 100,
                NumberOfStocks = 3,
                userId = 6,
                companyId = 7
            } };

            var sellRequests = new List<StockExchangeSellRequest> { new StockExchangeSellRequest
            {
                MinimumPerStock = 0,
                NumberOfStocks = 3,
                userId = 9,
                companyId = 7
            } };


            _context.StockExchangeBuyRequests.AddRange(buyRequests);
            _context.StockExchangeSellRequests.AddRange(sellRequests);

            _context.SaveChanges();

            var repo = new StocksRepository(_context);

            var sut = new StocksExchanger(_logger.Object, repo);

            //act
            sut.ExchangeStocksForCompany(company, time);

            //assert
            _context.StockExchangesCompleted.ToList().Count.ShouldBe(1);

            ((int)_context.StockExchangesCompleted.First().NumberOfStocks).ShouldBe(3);
            _context.StockExchangesCompleted.First().sellerId.ShouldBe(9);
            _context.StockExchangesCompleted.First().buyerId.ShouldBe(6);
            _context.StockExchangesCompleted.First().ExchangeDateAndTime.ShouldBe(time);
            _context.StockExchangesCompleted.First().companyId.ShouldBe(7);
            ((int)_context.StockExchangesCompleted.First().AmountPerStock).ShouldBe(100);

            _context.BankAccounts.Where(ba => ba.userId == 6).Single().Credit.ShouldBe(200);

            _context.BankAccounts.Where(ba => ba.userId == 9).Single().Credit.ShouldBe(300);
        }
    }
}
