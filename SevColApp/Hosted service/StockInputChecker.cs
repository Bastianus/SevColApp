using SevColApp.Models;
using SevColApp.Repositories;
using System.Collections.Generic;
using System.Linq;

namespace SevColApp.Hosted_service
{
    public class StockInputChecker : IStockInputChecker
    {
        private readonly IStocksRepository _repo;

        public StockInputChecker(IStocksRepository repo)
        {
            _repo = repo;
        }

        public void CheckAllSellRequestsAndRemoveInvalids()
        {
            var allCompanies = _repo.GetAllCompanies();

            allCompanies.ForEach(company =>
            {
                var allSellRequests = _repo.GetSellRequests(company).OrderBy(sr => sr.userId).ThenByDescending(sr => sr.MinimumPerStock).ToList();

                int i = 0;
                while (i < allSellRequests.Count)
                {
                    int currentUserId = allSellRequests[i].userId;

                    var currentSellRequests = allSellRequests.Where(sr => sr.userId == currentUserId).ToList();

                    if (_repo.UserHasNoBankAccount(currentUserId))
                    {
                        currentSellRequests.ForEach(sr => _repo.RemoveSellRequest(sr));
                        _repo.Save();
                    }
                    else
                    {
                        var stocksOwnedBySeller = _repo.AmountOfSellerStocks(currentUserId, company);

                        var totalStocksOfferedBySellerSoFar = 0;

                        for (int j = 0; j < currentSellRequests.Count; j++)
                        {
                            var currentSellRequest = currentSellRequests[j];

                            totalStocksOfferedBySellerSoFar += (int)currentSellRequest.NumberOfStocks;

                            if (totalStocksOfferedBySellerSoFar > stocksOwnedBySeller)
                            {
                                _repo.RemoveSellRequest(currentSellRequest);
                                _repo.Save();

                                totalStocksOfferedBySellerSoFar -= (int)currentSellRequest.NumberOfStocks;
                            }
                        }
                    }

                    i += currentSellRequests.Count;
                }
            });
        }

        public void CheckAllBuyRequestsAndRemoveInvalids()
        {
            var allCompanies = _repo.GetAllCompanies();

            var allBuyRequestForAllCompanies = new List<StockExchangeBuyRequest>();

            allCompanies.ForEach(company => allBuyRequestForAllCompanies.AddRange(_repo.GetBuyRequests(company)));

            allBuyRequestForAllCompanies = allBuyRequestForAllCompanies.OrderBy(br => br.userId).ThenBy(br => br.OfferPerStock).ToList();


            int i = 0;
            while (i < allBuyRequestForAllCompanies.Count)
            {
                int currentuserId = allBuyRequestForAllCompanies[i].userId;

                var currentBuyRequests = allBuyRequestForAllCompanies.Where(br => br.userId == currentuserId).ToList();

                if (_repo.UserHasNoBankAccount(currentuserId))
                {
                    currentBuyRequests.ForEach(br => _repo.RemoveBuyRequest(br));
                    _repo.Save();
                }
                else
                {
                    var buyerTotalCredits = _repo.UserTotalCredits(currentuserId);

                    long totalPossiblySpent = 0;

                    for (int j = 0; j < currentBuyRequests.Count; j++)
                    {
                        var currentBuyRequest = currentBuyRequests[j];

                        totalPossiblySpent += currentBuyRequest.NumberOfStocks * currentBuyRequest.OfferPerStock;

                        if (totalPossiblySpent > buyerTotalCredits)
                        {
                            _repo.RemoveBuyRequest(currentBuyRequest);
                            _repo.Save();

                            totalPossiblySpent -= currentBuyRequest.NumberOfStocks * currentBuyRequest.OfferPerStock;
                        }
                    }
                }

                i += currentBuyRequests.Count;
            }

        }
    }
}
