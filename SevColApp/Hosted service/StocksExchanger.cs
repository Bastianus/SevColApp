﻿using Microsoft.Extensions.Logging;
using SevColApp.Context;
using SevColApp.Helpers;
using SevColApp.Models;
using SevColApp.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SevColApp.Hosted_service
{
    public class StocksExchanger : IStocksExchanger
    {
        private readonly ILogger<SevColContext> _logger;
        private readonly IStocksRepository _repo;
        public StocksExchanger(ILogger<SevColContext> logger, IStocksRepository repo)
        {
            _logger = logger;
            _repo = repo;
        }

        public void ExchangeStocksForCompany(Company company, DateTime time)
        {
            var buyRequests = _repo.GetBuyRequests(company);

            var sellRequests = _repo.GetSellRequests(company);

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
                var remainingBuyRequests = _repo.GetBuyRequests(company);
                var remainingSellRequests = _repo.GetSellRequests(company);

                var offerBatchWithLowestRemainingMinimum = StocksHelper.GetBatchOfLowestMinimumOffers(remainingSellRequests);

                var amountOfStocksSold = SellStocksByBatch(offerBatchWithLowestRemainingMinimum, remainingBuyRequests, company, time);

                if (amountOfStocksSold <= 0) offersAreHigherThanRemainingMinimums = false;
            }

            return numberOfStocksTransferred;
        }

        private uint SellStocksByBatch(List<StockExchangeSellRequest> offers, List<StockExchangeBuyRequest> buys, Company company, DateTime time)
        {
            var amountPayedPerStock = StocksHelper.CalculateAmountPaidPerStock(offers, buys);

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

                if (_repo.SellerHasNoBankAccount(currentOffer.userId) || !_repo.SellerHasEnoughStocks(currentOffer.userId, company.Id, currentOffer.NumberOfStocks))
                {
                    sellIndex++;

                    continue;
                }

                if (!_repo.BuyerHasEnoughMoney(currentBuy, currentOffer.NumberOfStocks, price))
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

                    _repo.RemoveSellRequest(currentOffer);
                    _repo.RemoveBuyRequest(currentBuy);
                }
                else if (currentOffer.NumberOfStocks > currentBuy.NumberOfStocks)
                {
                    stocksExchanged = currentBuy.NumberOfStocks;

                    exchange.NumberOfStocks = stocksExchanged;

                    currentOffer.NumberOfStocks -= stocksExchanged;

                    buyIndex++;

                    _repo.UpdateSellRequest(currentOffer);
                    _repo.RemoveBuyRequest(currentBuy);
                }
                else
                {
                    stocksExchanged = currentOffer.NumberOfStocks;

                    exchange.NumberOfStocks = stocksExchanged;

                    currentBuy.NumberOfStocks -= stocksExchanged;

                    sellIndex++;

                    _repo.RemoveSellRequest(currentOffer);
                    _repo.UpdateBuyRequest(currentBuy);
                }

                _repo.ArrangePayment(currentBuy.userId, currentOffer.userId, stocksExchanged * price);

                _repo.TransferStocks(currentBuy.userId, currentOffer.userId, company.Id, stocksExchanged);

                numberSold += stocksExchanged;

                _repo.AddCompletedExchange(exchange);

                _repo.Save();
            }

            return numberSold;
        } 
    }
}