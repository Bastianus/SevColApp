using SevColApp.Models;
using System.Collections.Generic;
using System.Linq;

namespace SevColApp.Helpers
{
    public static class StocksHelper
    {
        public static uint CalculateAmountPaidPerStock(List<StockExchangeSellRequest> offers, List<StockExchangeBuyRequest> buys)
        {
            var stocksOnOffer = CalculateNumberOfStocksInBatch(offers);

            uint answer = 0;

            int i = 0;
            bool validBuys = true;
            while (validBuys)
            {
                if (buys[i].OfferPerStock >= offers.First().MinimumPerStock)
                {
                    stocksOnOffer -= (int)buys[i].NumberOfStocks;
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

        public static List<StockExchangeSellRequest> GetBatchOfLowestMinimumOffers(List<StockExchangeSellRequest> offers)
        {
            offers = offers.Where(o => o.NumberOfStocks > 0).OrderBy(o => o.MinimumPerStock).ToList();
            var currentLowestMinimum = offers.First().MinimumPerStock;

            return offers.Where(o => o.MinimumPerStock == currentLowestMinimum).ToList();
        }

        private static int CalculateNumberOfStocksInBatch(List<StockExchangeSellRequest> offers)
        {
            int totalNumberOfStocks = 0;

            foreach (var offer in offers) totalNumberOfStocks += (int)offer.NumberOfStocks;

            return totalNumberOfStocks;
        }
    }
}
