using Microsoft.Extensions.Logging;
using SevColApp.Context;
using System;
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

                await Task.Delay(TimeSpan.FromSeconds(15), token);
            }            
        }
    }
}
