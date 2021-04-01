using Microsoft.Extensions.Logging;
using SevColApp.Context;
using SevColApp.Models;
using SevColApp.Repositories;
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
        private readonly IStocksRepository _repo;
        private readonly IStocksExchanger _stocksExchanger;
        private readonly IStockInputChecker _stockInputChecker;

        public StockExchange(ILogger<SevColContext> logger, IStocksRepository repo, IStocksExchanger stocksExchanger, IStockInputChecker stockInputChecker)
        {
            _logger = logger;
            _repo = repo;
            _stocksExchanger = stocksExchanger;
            _stockInputChecker = stockInputChecker;
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

                await Task.Delay(TimeSpan.FromMinutes(15), token);
            }
        }        
    }
}
