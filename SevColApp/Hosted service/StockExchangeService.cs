using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
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

                await action.ExchangeStocks(stoppingToken);
            }
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation($"Stock excahnge service is stopping at {DateTime.Now}.");

            await base.StopAsync(stoppingToken);
        }
    }
}
