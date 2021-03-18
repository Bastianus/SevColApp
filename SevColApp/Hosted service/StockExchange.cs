using Microsoft.Extensions.Logging;
using SevColApp.Context;
using SevColApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SevColApp.Hosted_service
{
    public class StockExchange : IStockExchange
    {
        private readonly ILogger _logger;
        private readonly SevColContext _context;

        public StockExchange(ILogger<SevColContext> logger, SevColContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task ExchangeStocks(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                _logger.LogInformation($"Exchange of stocks started at {DateTime.Now}.");

                var allCompanies = GetAllCompanies();

                foreach (Company company in allCompanies)
                {
                    var startTime = DateTime.Now;
                    ExchangeStocksForCompany(company, startTime);
                }

                await Task.Delay(TimeSpan.FromMinutes(15), token);
            }
        }

        private void ExchangeStocksForCompany(Company company, DateTime time)
        {
            var buyRequests = GetBuyRequests(company);

            var sellRequests = GetSellRequests(company);

            if (buyRequests == null || sellRequests == null)
            {
                _logger.LogInformation($"For company {company.Name} there were no buy request and/or no sell requests.");
                return;
            }

            var stocksTransferred = MatchSellersAndBuyers(company, time);

            _logger.LogInformation($"For company {company.Name} a total of {stocksTransferred} stocks were transferred.");
        }

        private int MatchSellersAndBuyers(Company company, DateTime time)
        {
            int numberOfStocksTransferred = 0;

            bool offersAreHigherThanRemainingMinimums = true;


            while (offersAreHigherThanRemainingMinimums)
            {
                var remainingBuyRequests = GetBuyRequests(company);
                var remainingSellRequests = GetSellRequests(company);

                var offerBatchWithLowestRemainingMinimum = GetBatchOfLowestMinimumOffers(remainingSellRequests);

                var amountOfStocksSold = SellStocksByBatch(offerBatchWithLowestRemainingMinimum, remainingBuyRequests, company, time);

                if (amountOfStocksSold <= 0) offersAreHigherThanRemainingMinimums = false;
            }

            return numberOfStocksTransferred;
        }

        private uint SellStocksByBatch(List<StockExchangeSellRequest> offers, List<StockExchangeBuyRequest> buys, Company company, DateTime time)
        {
            var amountPayedPerStock = CalculateAmountPaidPerStock(offers, buys);

            if (amountPayedPerStock <= 0)
            {
                return 0;
            }

            return MakeTheExchanges(offers, buys, amountPayedPerStock, company, time);
        }

        private uint MakeTheExchanges(List<StockExchangeSellRequest> sellRequestBatch, List<StockExchangeBuyRequest> buyRequests, uint price, Company company, DateTime time)
        {
            int sellIndex = 0;
            int buyIndex = 0;

            uint numberSold = 0;

            while (sellIndex < sellRequestBatch.Count && buyIndex < buyRequests.Count && sellRequestBatch[sellIndex].MinimumPerStock <= buyRequests[buyIndex].OfferPerStock)
            {
                var currentOffer = sellRequestBatch[sellIndex];
                var currentBuy = buyRequests[buyIndex];

                var exchange = new StockExchangeCompleted
                {
                    AmountPerStock = price,
                    companyId = company.Id,
                    sellerId = currentOffer.userId,
                    buyerId = currentBuy.userId,
                    ExchangeDateAndTime = time
                };

                if (SellerHasNoBankAccount(currentOffer.userId) || !SellerHasEnoughStocks(currentOffer.userId, company.Id, currentOffer.NumberOfStocks))
                {
                    sellIndex++;

                    continue;
                }

                if(!BuyerHasEnoughMoney(currentBuy, currentOffer.NumberOfStocks, price))
                {
                    buyIndex++;

                    continue;
                }

                uint stocksExchanged;

                if (currentOffer.NumberOfStocks == currentBuy.NumberOfStocks)
                {
                    stocksExchanged = currentOffer.NumberOfStocks;

                    exchange.NumberOfStocks = stocksExchanged;

                    sellIndex++;
                    buyIndex++;

                    _context.StockExchangeSellRequests.Remove(currentOffer);
                    _context.StockExchangeBuyRequests.Remove(currentBuy);
                }
                else if (currentOffer.NumberOfStocks > currentBuy.NumberOfStocks)
                {
                    stocksExchanged = currentBuy.NumberOfStocks;                    

                    exchange.NumberOfStocks = stocksExchanged;

                    currentOffer.NumberOfStocks -= stocksExchanged;

                    buyIndex++;

                    _context.StockExchangeSellRequests.Update(currentOffer);
                    _context.StockExchangeBuyRequests.Remove(currentBuy);
                }
                else
                {
                    stocksExchanged = currentOffer.NumberOfStocks;

                    exchange.NumberOfStocks = stocksExchanged;

                    currentBuy.NumberOfStocks -= stocksExchanged;

                    sellIndex++;

                    _context.StockExchangeSellRequests.Remove(currentOffer);
                    _context.StockExchangeBuyRequests.Update(currentBuy);
                }

                ArrangePayment(currentBuy.userId, currentOffer.userId, stocksExchanged * price);

                TransferStocks(currentBuy.userId, currentOffer.userId, company.Id, stocksExchanged);

                numberSold += stocksExchanged;

                _context.StockExchangesCompleted.Add(exchange);

                _context.SaveChanges();
            }

            return numberSold;
        }

        private void TransferStocks(int buyerId, int sellerId, int companyId, uint numberOfStocksTransferred) 
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

        private void ArrangePayment(int buyerId, int sellerId, long priceToPay)
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

        private static uint CalculateAmountPaidPerStock(List<StockExchangeSellRequest> offers, List<StockExchangeBuyRequest> buys)
        {
            var stocksOnOffer = CalculateNumberOfStocksInBatch(offers);

            uint answer = 0;

            int i = 0;
            bool validBuys = true;
            while (validBuys)
            {
                if (buys[i].OfferPerStock >= offers.First().MinimumPerStock)
                {
                    stocksOnOffer -= buys[i].NumberOfStocks;
                }
                else
                {
                    validBuys = false;
                }

                if (stocksOnOffer <= 0 || !validBuys)
                {
                    answer = buys[i].OfferPerStock;
                    break;
                }

                i++;
            }

            return answer;
        }

        private static uint CalculateNumberOfStocksInBatch(List<StockExchangeSellRequest> offers)
        {
            uint totalNumberOfStocks = 0;

            foreach (var offer in offers) totalNumberOfStocks += offer.NumberOfStocks;

            return totalNumberOfStocks;
        }

        private bool SellerHasEnoughStocks(int sellerId, int companyId, uint numberOfStocks)
        {
            return _context.UserCompanyStocks.Any(uc => uc.userId == sellerId && uc.companyId == companyId)
                && _context.UserCompanyStocks.Where(uc => uc.userId == sellerId && uc.companyId == companyId).First().NumberOfStocks >= numberOfStocks;
        }

        private bool SellerHasNoBankAccount(int sellerId)
        {
            return !_context.BankAccounts.Any(ba => ba.userId == sellerId);
        }

        private bool BuyerHasEnoughMoney(StockExchangeBuyRequest buyerRequest, uint numberOfStocks, uint pricePerStock)
        {
            var buyersTotalCredit = _context.BankAccounts.Where(ba => ba.userId == buyerRequest.userId)?.Select(ba => ba.Credit).Sum();

            if (buyersTotalCredit == null) buyersTotalCredit = 0;

            var totalPrice = numberOfStocks * pricePerStock;

            return buyersTotalCredit >= totalPrice;
        }

        private List<Company> GetAllCompanies()
        {
            return _context.Companies.ToList();
        }

        private List<StockExchangeBuyRequest> GetBuyRequests(Company company)
        {
            return _context.StockExchangeBuyRequests.Where(br => br.companyId == company.Id).OrderByDescending(br => br.OfferPerStock).ThenBy(br => br.NumberOfStocks).ToList();
        }

        private List<StockExchangeSellRequest> GetSellRequests(Company company)
        {
            return _context.StockExchangeSellRequests.Where(br => br.companyId == company.Id).OrderBy(br => br.MinimumPerStock).ThenBy(br => br.NumberOfStocks).ToList();
        }
        private static List<StockExchangeSellRequest> GetBatchOfLowestMinimumOffers(List<StockExchangeSellRequest> offers)
        {
            offers = offers.Where(o => o.NumberOfStocks > 0).OrderBy(o => o.MinimumPerStock).ToList();
            var currentLowestMinimum = offers.First().MinimumPerStock;

            return offers.Where(o => o.MinimumPerStock == 0).ToList();
        }
    }
}
