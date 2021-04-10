using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SevColApp.Helpers;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SevColApp.Hosted_service
{
    public class StockExchangeService : BackgroundService
    {
        private readonly ILogger _logger;

        public IServiceProvider Services { get; }

        public StockExchangeService(IServiceProvider services, ILogger<StockExchangeService> logger)
        {
            Services = services;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation($"Stock exchange service started at {DateTime.Now}.");

            await DoWork(stoppingToken);
        }

        private async Task DoWork(CancellationToken stoppingToken)
        {
            _logger.LogInformation($"Stock exchange started it's work on {DateTime.Now}.");

            using (var scope = Services.CreateScope())
            {
                var action = scope.ServiceProvider.GetRequiredService<IStockExchange>();

                await WaitForHourDevisibleByThree(stoppingToken);

                await action.ExchangeStocks(stoppingToken);                
            }
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation($"Stock excahnge service is stopping at {DateTime.Now}.");

            await base.StopAsync(stoppingToken);
        }

        private async Task WaitForHourDevisibleByThree(CancellationToken stoppingToken)
        {
            var now = DateTime.Now;

            var hoursToWait = 3 - now.Hour % 3;

            var minutesToWait = hoursToWait * 60 - now.Minute;

            _logger.LogInformation($"Stock exchange service waiting for {minutesToWait} minutes.");

            await Task.Delay(TimeSpan.FromMinutes(minutesToWait), stoppingToken);
        }        
    }
}
