using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SevColApp.Helpers;
using SevColApp.Models;
using SevColApp.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SevColApp.Controllers
{
    public class GamemasterStockExchangeController : Controller
    {
        private readonly ILogger<GamemasterController> _logger;
        private readonly IGamemasterStockExchangeRepository _repo;
        private readonly IStocksRepository _stocksRepo;
        private readonly CookieHelper _cookieHelper;

        public GamemasterStockExchangeController(ILogger<GamemasterController> logger, IGamemasterStockExchangeRepository repo, IStocksRepository stocksRepo, CookieHelper cookieHelper)
        {
            _logger = logger;
            _repo = repo;
            _stocksRepo = stocksRepo;
            _cookieHelper = cookieHelper;
        }

        public IActionResult Index()
        {
            if (!_cookieHelper.IsThereAGameMasterCookie())
            {
                return RedirectToAction("Login", "Home");
            }

            return View();
        }

        public IActionResult GlobalVariables()
        {
            if (!_cookieHelper.IsThereAGameMasterCookie())
            {
                return RedirectToAction("Login", "Home");
            }

            var input = new Global
            {
                Errors = new List<string>(),
                MarketTrendFactor = _repo.GetGlobalTrend(),
                MarketVolatility = _repo.GetGlobalVolatility()
            };

            return View(input);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult GlobalVariables(Global input)
        {
            input.Errors = new List<string>();

            var newMarketTrendValue = ChangeInputStringsToFloats(input.InputStringTrend);
            var newMarketVolatilityValue = ChangeInputStringsToFloats(input.InputStringVolatility);

            if(input.Errors.Count == 0)
            {
                _repo.SetGlobalTrend(newMarketTrendValue);
                _repo.SetGlobalVolatility(newMarketVolatilityValue);
            }

            return View("GlobalVariables", input);

            float ChangeInputStringsToFloats(string inputString)
            {
                float output;
                bool parseable;

                var indexOfSeparator = inputString.IndexOf(".");

                if (indexOfSeparator == -1)
                {
                    indexOfSeparator = inputString.IndexOf(",");
                }

                if(indexOfSeparator == -1)
                {
                    parseable = float.TryParse(inputString, System.Globalization.NumberStyles.Float, null, out output);
                }
                else
                {
                    var localInputString = inputString.Substring(0, indexOfSeparator) + "," + inputString.Substring(indexOfSeparator+1);
                    parseable = float.TryParse(localInputString, System.Globalization.NumberStyles.Float, null, out output);
                }

                if (!parseable)
                {
                    input.Errors.Add($"The input value \"{ inputString}\" could not be parsed to a float.");
                    return 1;
                }
                else
                {
                    return output;
                }
            }
        }
    }
}
