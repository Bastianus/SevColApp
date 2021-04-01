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
        private readonly IHomeRepository _userRepo;
        private readonly ILogger<StocksController> _logger;
        private readonly CookieHelper _cookieHelper;

        public StocksController(ILogger<StocksController> logger, IStocksRepository repo, IHomeRepository userRepo, CookieHelper cookieHelper)
        {
            _logger = logger;
            _repo = repo;
            _userRepo = userRepo;
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

            var userName = _userRepo.FindUserById(id).FullName;

            return View("Index", userName);
        }

    }
}
