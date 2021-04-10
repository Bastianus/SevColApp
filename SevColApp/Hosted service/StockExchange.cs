using Microsoft.Extensions.Logging;
using SevColApp.Context;
using SevColApp.Helpers;
using SevColApp.Models;
using SevColApp.Repositories;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SevColApp.Hosted_service
{
    public class StockExchange : IStockExchange
    {
        private readonly ILogger _logger;
        private readonly IStocksRepository _repo;
        private readonly IStocksExchanger _stocksExchanger;
        private readonly IStockInputChecker _stockInputChecker;
        private readonly TimeHelper _timeHelper;

        public StockExchange(ILogger<SevColContext> logger, IStocksRepository repo, IStocksExchanger stocksExchanger, IStockInputChecker stockInputChecker, TimeHelper timeHelper)
        {
            _logger = logger;
            _repo = repo;
            _stocksExchanger = stocksExchanger;
            _stockInputChecker = stockInputChecker;
            _timeHelper = timeHelper;
        }

        public async Task ExchangeStocks(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                _logger.LogInformation($"Exchange of stocks started at {DateTime.Now}.");

                _stockInputChecker.CheckAllBuyRequestsAndRemoveInvalids();
                _stockInputChecker.CheckAllSellRequestsAndRemoveInvalids();

                var allCompanies = _repo.GetAllCompanies();

                foreach (Company company in allCompanies)
                {
                    var startTime = DateTime.Now;
                    _stocksExchanger.ExchangeStocksForCompany(company, startTime);
                }

                _stocksExchanger.RemoveAllRemainingRequests();

                _logger.LogInformation("Stock exchanger completed its task");

                await _timeHelper.WaitForHourDevisibleByThree(_logger, token);
            }
        }
    }
}
