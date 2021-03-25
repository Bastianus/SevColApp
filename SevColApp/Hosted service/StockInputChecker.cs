using SevColApp.Repositories;
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
                while(i < allSellRequests.Count)
                {
                    int currentUserId = allSellRequests[i].userId;

                    var currentSellRequests = allSellRequests.Where(sr => sr.userId == currentUserId).ToList();

                    if (_repo.UserHasNoBankAccount(currentUserId))
                    {
                        currentSellRequests.ForEach(sr => _repo.RemoveSellRequest(sr));
                    }
                    else
                    {
                        var stocksOwnedBySeller = _repo.AmountOfSellerStocks(currentUserId, company);

                        var totalStocksOfferedBySellerSoFar = 0;

                        for (int j = 0; j < currentSellRequests.Count;)
                        {
                            var currentSellRequest = currentSellRequests[j];

                            totalStocksOfferedBySellerSoFar += (int)currentSellRequest.NumberOfStocks;

                            if (totalStocksOfferedBySellerSoFar > stocksOwnedBySeller)
                            {
                                totalStocksOfferedBySellerSoFar -= (int)currentSellRequest.NumberOfStocks;

                                _repo.RemoveSellRequest(currentSellRequest);
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

            allCompanies.ForEach(company =>
            {
                var allBuyRequests = _repo.GetBuyRequests(company).OrderBy(br => br.userId).ThenBy(br => br.OfferPerStock).ToList();

                int i = 0;
                while(i < allBuyRequests.Count)
                {
                    int currentuserId = allBuyRequests[i].userId;

                    var currentBuyRequests = allBuyRequests.Where(br => br.userId == currentuserId).ToList();

                    if (_repo.UserHasNoBankAccount(currentuserId))
                    {
                        currentBuyRequests.ForEach(br => _repo.RemoveBuyRequest(br));
                    }
                    else
                    {
                        var buyerTotalCredits = _repo.UserTotalCredits(currentuserId);

                        long totalPossiblySpent = 0;

                        for(int j = 0;j < currentBuyRequests.Count; j++)
                        {
                            var currentBuyRequest = currentBuyRequests[j];

                            totalPossiblySpent += currentBuyRequest.NumberOfStocks * currentBuyRequest.OfferPerStock;

                            if(totalPossiblySpent > buyerTotalCredits)
                            {
                                _repo.RemoveBuyRequest(currentBuyRequest);

                                totalPossiblySpent -= currentBuyRequest.NumberOfStocks * currentBuyRequest.OfferPerStock;
                            }
                        }
                    }

                    i += currentBuyRequests.Count;
                }
            });
        }
    }
}
