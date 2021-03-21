using SevColApp.Context;
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

        public List<StockExchangeBuyRequest> GetBuyRequests(Company company)
        {
            return _context.StockExchangeBuyRequests.Where(br => br.companyId == company.Id).OrderByDescending(br => br.OfferPerStock).ThenBy(br => br.NumberOfStocks).ToList();
        }

        public List<StockExchangeSellRequest> GetSellRequests(Company company)
        {
            return _context.StockExchangeSellRequests.Where(br => br.companyId == company.Id).OrderBy(br => br.MinimumPerStock).ThenBy(br => br.NumberOfStocks).ToList();
        }

        public bool BuyerHasEnoughMoney(StockExchangeBuyRequest buyerRequest, uint numberOfStocks, uint pricePerStock)
        {
            var buyersTotalCredit = _context.BankAccounts.Where(ba => ba.userId == buyerRequest.userId)?.Select(ba => ba.Credit).Sum();

            if (buyersTotalCredit == null) buyersTotalCredit = 0;

            var totalPrice = numberOfStocks * pricePerStock;

            return buyersTotalCredit >= totalPrice;
        }

        public bool SellerHasEnoughStocks(int sellerId, int companyId, uint numberOfStocks)
        {
            return _context.UserCompanyStocks.Any(uc => uc.userId == sellerId && uc.companyId == companyId)
                && _context.UserCompanyStocks.Where(uc => uc.userId == sellerId && uc.companyId == companyId).First().NumberOfStocks >= numberOfStocks;
        }

        public bool SellerHasNoBankAccount(int sellerId)
        {
            return !_context.BankAccounts.Any(ba => ba.userId == sellerId);
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
        public void Save()
        {
            _context.SaveChanges();
        }
    }
}
