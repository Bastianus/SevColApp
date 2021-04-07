﻿using SevColApp.Context;
using SevColApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SevColApp.Repositories
{
    public class StocksRepository : IStocksRepository
    {
        private readonly SevColContext _context;

        public StocksRepository(SevColContext context)
        {
            _context = context;
        }

        public List<Company> GetAllCompanies()
        {
            return _context.Companies.ToList();
        }

        public StockExchangeBuyRequest AddBuyRequest(StockExchangeBuyRequest request)
        {  
            if (!CompanyExists(request.CompanyName)) request.Errors.Add($"The company {request.CompanyName} does not exist.");

            if (UserHasNoBankAccount(request.userId)) request.Errors.Add($"You need a bank account.");
            else if (!UserHasEnoughMoney(request.userId, request.NumberOfStocks, request.OfferPerStock)) request.Errors.Add($"You do not have {request.NumberOfStocks * request.OfferPerStock} credits on your bank accounts to pay for this request.");

            if(request.Errors.Count == 0)
            {
                _context.StockExchangeBuyRequests.Add(request);
            }

            return request;
        }

        public StockExchangeSellRequest AddSellRequest(StockExchangeSellRequest request)
        {
            if (!CompanyExists(request.CompanyName)) request.Errors.Add($"The company {request.CompanyName} does not exist.");
            else if (!UserHasEnoughStocksFromCompany(request.CompanyName, request.NumberOfStocks)) request.Errors.Add($"You do not have enough stocks from {request.CompanyName}.");

            if (UserHasNoBankAccount(request.userId)) request.Errors.Add($"You need a bank account.");

            if(request.Errors.Count == 0)
            {
                _context.StockExchangeSellRequests.Add(request);
            }

            return request;
        }

        public List<StockExchangeBuyRequest> GetBuyRequests(Company company)
        {
            return _context.StockExchangeBuyRequests.Where(br => br.companyId == company.Id).OrderByDescending(br => br.OfferPerStock).ThenBy(br => br.NumberOfStocks).ToList();
        }

        public List<StockExchangeSellRequest> GetSellRequests(Company company)
        {
            return _context.StockExchangeSellRequests.Where(br => br.companyId == company.Id).OrderBy(br => br.MinimumPerStock).ThenBy(br => br.NumberOfStocks).ToList();
        }

        public long UserTotalCredits(int userId)
        {
            return _context.BankAccounts.Where(ba => ba.userId == userId)?.Select(ba => ba.Credit).Sum() ?? 0;
        }

        public uint AmountOfSellerStocks(int sellerId, Company company)
        {
            if (!_context.UserCompanyStocks.Any(uc => uc.companyId == company.Id && uc.userId == sellerId)) return 0;

            return _context.UserCompanyStocks.Where(uc => uc.companyId == company.Id && uc.userId == sellerId).Single().NumberOfStocks;
        }

        public bool UserHasNoBankAccount(int userId)
        {
            return !_context.BankAccounts.Any(ba => ba.userId == userId);
        }

        public void TransferStocks(int buyerId, int sellerId, int companyId, uint numberOfStocksTransferred)
        {
            var sellerStocks = _context.UserCompanyStocks.Where(uc => uc.userId == sellerId && uc.companyId == companyId).First();

            sellerStocks.NumberOfStocks -= numberOfStocksTransferred;

            _context.UserCompanyStocks.Update(sellerStocks);

            var buyerStocks = _context.UserCompanyStocks.Where(uc => uc.userId == buyerId && uc.companyId == companyId).FirstOrDefault();

            if (buyerStocks == null)
            {
                buyerStocks = new UserCompanyStocks
                {
                    userId = buyerId,
                    companyId = companyId,
                    NumberOfStocks = numberOfStocksTransferred
                };

                _context.UserCompanyStocks.Add(buyerStocks);
            }
            else
            {
                buyerStocks.NumberOfStocks += numberOfStocksTransferred;

                _context.UserCompanyStocks.Update(buyerStocks);
            }
        }

        public void ArrangePayment(int buyerId, int sellerId, long priceToPay)
        {
            var receivingAccount = _context.BankAccounts.Where(ba => ba.userId == sellerId).OrderByDescending(ba => ba.Credit).First();

            var accountWithMostMoney = _context.BankAccounts.Where(ba => ba.userId == buyerId).OrderByDescending(ba => ba.Credit).First();

            if (accountWithMostMoney.Credit < priceToPay)
            {
                priceToPay -= accountWithMostMoney.Credit;

                receivingAccount.Credit += accountWithMostMoney.Credit;

                accountWithMostMoney.Credit = 0;

                _context.BankAccounts.Update(accountWithMostMoney);
                _context.BankAccounts.Update(receivingAccount);

                _context.SaveChanges();

                ArrangePayment(buyerId, sellerId, priceToPay);
            }

            accountWithMostMoney.Credit -= priceToPay;
            receivingAccount.Credit += priceToPay;

            _context.BankAccounts.Update(accountWithMostMoney);
            _context.BankAccounts.Update(receivingAccount);
        }

        public void RemoveSellRequest(StockExchangeSellRequest sellRequest)
        {
            _context.StockExchangeSellRequests.Remove(sellRequest);
        }

        public void RemoveBuyRequest(StockExchangeBuyRequest buyRequest)
        {
            _context.StockExchangeBuyRequests.Remove(buyRequest);
        }

        public void UpdateSellRequest(StockExchangeSellRequest sellRequest)
        {
            _context.StockExchangeSellRequests.Update(sellRequest);
        }
        public void UpdateBuyRequest(StockExchangeBuyRequest buyRequest)
        {
            _context.StockExchangeBuyRequests.Update(buyRequest);
        }
        public void AddCompletedExchange(StockExchangeCompleted exchange)
        {
            _context.StockExchangesCompleted.Add(exchange);
        }

        public UsersCurrentStocks GetStocksFromUser(int id)
        {
            var answer = new UsersCurrentStocks();

            foreach(var company in _context.Companies)
            {
                var userStocksInCompanyBought = _context.StockExchangesCompleted.Where(sec => sec.companyId == company.Id && sec.buyerId == id).Sum(sec => sec.NumberOfStocks);

                var userStocksInCompanySold = _context.StockExchangesCompleted.Where(sec => sec.companyId == company.Id && sec.sellerId == id).Sum(sec => sec.NumberOfStocks);

                var userStocksInCompany = userStocksInCompanyBought - userStocksInCompanySold;

                answer.UserStocks.Add(new UserStocks { Company = company, NumberofStocks = userStocksInCompany });                
            }

            answer.UserStocks.OrderByDescending(us => us.NumberofStocks).ThenBy(us => us.Company.Name);

            return answer;
        }

        public void Save()
        {
            _context.SaveChanges();
        }

        private bool CompanyExists(string companyName)
        {
            return _context.Companies.Any(c => c.Name == companyName);
        }

        private bool UserHasEnoughMoney(int userId, uint numberOfStocks, uint offerPerStock)
        {
            return UserTotalCredits(userId) >= numberOfStocks * offerPerStock;
        }

        private bool UserHasEnoughStocksFromCompany(string companyName, uint numberOfStocksOffered)
        {
            var companyId = _context.Companies.First(c => c.Name == companyName).Id;

            var userStocksOfCompany = _context.UserCompanyStocks.FirstOrDefault(ucs => ucs.companyId == companyId);

            if (userStocksOfCompany == null) return false;

            return userStocksOfCompany.NumberOfStocks >= numberOfStocksOffered;
        }
    }
}
