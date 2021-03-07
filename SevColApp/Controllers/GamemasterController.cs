using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SevColApp.Helpers;
using SevColApp.Models;
using SevColApp.Repositories;
using System.Collections.Generic;

namespace SevColApp.Controllers
{
    public class GamemasterController : Controller
    {
        private readonly ILogger<GamemasterController> _logger;
        private readonly IGamemasterRepository _repo;
        private readonly CookieHelper _cookieHelper;

        public GamemasterController(ILogger<GamemasterController> logger, IGamemasterRepository repo, CookieHelper cookieHelper)
        {
            _logger = logger;
            _repo = repo;
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

        public IActionResult Users()
        {
            if (!_cookieHelper.IsThereAGameMasterCookie())
            {
                return RedirectToAction("Login", "Home");
            }

            var answer = new AllUsers { Users = _repo.GetAllUsers() };

            return View(answer);
        }

        public IActionResult PayUser(string userLoginName)
        {
            var answer = _repo.PayAllowanceForUser(userLoginName);

            return View("Users", answer);
        }

        public IActionResult AllowanceReset()
        {
            var answer = _repo.ResetAllowances();

            return View("Users", answer);
        }

        public IActionResult EnterUserName()
        {
            if (!_cookieHelper.IsThereAGameMasterCookie())
            {
                return RedirectToAction("Login", "Home");
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EnterUserName(User user)
        {
            var answer = _repo.GetAllAccountsOfUser(user.LoginName);

            return View("UserBankAccounts", answer);
        }

        public IActionResult ChangeUserPassword()
        {
            if (!_cookieHelper.IsThereAGameMasterCookie())
            {
                return RedirectToAction("Login", "Home");
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ChangeUserPassword(UserPasswordChange input)
        {
            if (!ModelState.IsValid)
            {
                RedirectToAction("Index");
            }

            var result = _repo.ChangeUserPassword(input);

            return View("UserPasswordChanged", result);
        }

        public IActionResult ChangeBankAccountPassword()
        {
            if (!_cookieHelper.IsThereAGameMasterCookie())
            {
                return RedirectToAction("Login", "Home");
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ChangeBankAccountPassword(AccountPasswordChange input)
        {
            if (!ModelState.IsValid)
            {
                RedirectToAction("Index");
            }

            var result = _repo.ChangeBankAccountPassword(input);

            return View("BankAccountPasswordChanged", result);
        }
    }
}
