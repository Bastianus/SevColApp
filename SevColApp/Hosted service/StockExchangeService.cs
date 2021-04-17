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
        private readonly TimeHelper _timeHelper;

        public IServiceProvider Services { get; }

        public StockExchangeService(IServiceProvider services, ILogger<StockExchangeService> logger, TimeHelper timeHelper)
        {
            Services = services;
            _logger = logger;
            _timeHelper = timeHelper;
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

                await _timeHelper.WaitForHourDevisibleByThree(_logger, stoppingToken);

                await action.ExchangeStocks(stoppingToken);                
            }
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation($"Stock exchange service is stopping at {DateTime.Now}.");

            await base.StopAsync(stoppingToken);
        }    
    }
}
