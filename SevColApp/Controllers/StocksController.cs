using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SevColApp.Helpers;
using SevColApp.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SevColApp.Controllers
{
    public class StocksController : Controller
    {
        private readonly IStocksRepository _repo;
        private readonly ILogger<StocksController> _logger;
        private readonly CookieHelper _cookieHelper;

        public StocksController(ILogger<StocksController> logger, IStocksRepository repo, CookieHelper cookieHelper)
        {
            _logger = logger;
            _repo = repo;
            _cookieHelper = cookieHelper;

            _logger.LogInformation("Stocks controller started");
        }

        public IActionResult Index()
        {
            if (!_cookieHelper.IsThereACookie())
            {
                return RedirectToAction("Login", "Home");
            }

            var id = _cookieHelper.GetUserIdFromCookie();

            return View();

        }

    }
}
