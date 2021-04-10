using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SevColApp.Helpers;
using SevColApp.Hosted_service;
using System.IO;

namespace SevColApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseUrls("https://*:5001");
                    webBuilder.UseContentRoot(Directory.GetCurrentDirectory());
                    webBuilder.UseIISIntegration();
                    webBuilder.UseStartup<Startup>();
                })
                .ConfigureServices(services =>
               {
                   //services.AddSingleton<TimeHelper>();
                   //services.AddHostedService<StockExchangeService>();
                   //services.AddScoped<IStockExchange, StockExchange>();
               });
    }
}
