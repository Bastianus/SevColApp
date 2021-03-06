﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SevColApp.Helpers;
using SevColApp.Models;
using SevColApp.Repositories;
using System.Linq;

namespace SevColApp.Controllers
{
    public class StocksController : Controller
    {
        private readonly IStocksRepository _repo;
        private readonly IHomeRepository _userRepo;
        private readonly IBankRepository _bankRepo;
        private readonly ILogger<StocksController> _logger;
        private readonly CookieHelper _cookieHelper;

        public StocksController(ILogger<StocksController> logger, IStocksRepository repo, IHomeRepository userRepo, IBankRepository bankRepo, CookieHelper cookieHelper)
        {
            _logger = logger;
            _repo = repo;
            _userRepo = userRepo;
            _bankRepo = bankRepo;
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

        public IActionResult CurrentStocks()
        {
            if (!_cookieHelper.IsThereACookie())
            {
                return RedirectToAction("Login", "Home");
            }

            var id = _cookieHelper.GetUserIdFromCookie();

            var usersStocks = _repo.GetStocksFromUser(id);

            return View(usersStocks);
        }

        public IActionResult BuyRequest()
        {
            if (!_cookieHelper.IsThereACookie())
            {
                return RedirectToAction("Login", "Home");
            }

            var userId = _cookieHelper.GetUserIdFromCookie();

            var input = new BuyRequestInputOutput
            {
                Companies = _repo.GetAllCompanies(),
                BankAccounts = _repo.GetBankAccountsFromUser(userId),
                BuyRequest = new StockExchangeBuyRequest
                {
                    userId = userId
                }
            };

            return View("BuyRequest", input);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult BuyRequest(BuyRequestInputOutput input)
        {
            StockExchangeBuyRequest answer;

            if(_bankRepo.IsAccountPasswordCorrect(input.BuyRequest.AccountNumber, input.BuyRequest.Password))
            {
                answer = _repo.AddBuyRequest(input.BuyRequest);
            }
            else
            {
                answer = input.BuyRequest;

                answer.Errors.Add($"The password for the account with account number {input.BuyRequest.AccountNumber} is incorrect or the account number is unknown.");
            }            

            return View("BuyRequestResults", answer);
        }

        public IActionResult SellRequest()
        {
            if (!_cookieHelper.IsThereACookie())
            {
                return RedirectToAction("Login", "Home");
            }

            var userId = _cookieHelper.GetUserIdFromCookie();

            var userStocks = _repo.GetStocksFromUser(userId);

            var input = new SellRequestInputOutput
            {
                UsersCurrentStocks = userStocks,
                BankAccounts = _repo.GetBankAccountsFromUser(userId),
                SellRequest = new StockExchangeSellRequest { userId = userId }
            };

            return View("SellRequest", input);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SellRequest(SellRequestInputOutput request)
        {
            StockExchangeSellRequest answer;

            if (!_cookieHelper.IsThereACookie())
            {
                return RedirectToAction("Login", "Home");
            }

            var userId = _cookieHelper.GetUserIdFromCookie();

            var possibleBankAccounts = _repo.GetBankAccountsFromUser(userId);

            if (possibleBankAccounts.Any(ba => ba.AccountNumber == request.SellRequest.AccountNumber))
            {
                answer = _repo.AddSellRequest(request.SellRequest);
            }
            else
            {
                answer = request.SellRequest;

                answer.Errors.Add($"The account number {request.SellRequest.AccountNumber} is not registered for this user or does not exist.");
            }            
            
            return View("BuyRequestResults", answer);
        }

        public IActionResult Test()
        {
            return View();
        }
    }
}
