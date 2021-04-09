using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SevColApp.Helpers;
using SevColApp.Models;
using SevColApp.Repositories;
using System.Collections.Generic;
using System.Linq;

namespace SevColApp.Controllers
{
    public class GamemasterController : Controller
    {
        private readonly ILogger<GamemasterController> _logger;
        private readonly IGamemasterRepository _repo;
        private readonly IStocksRepository _stocksRepo;
        private readonly CookieHelper _cookieHelper;

        public GamemasterController(ILogger<GamemasterController> logger, IGamemasterRepository repo, IStocksRepository stocksRepo, CookieHelper cookieHelper)
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
            if (!_cookieHelper.IsThereAGameMasterCookie())
            {
                return RedirectToAction("Login", "Home");
            }

            var answer = _repo.PayAllowanceForUser(userLoginName);

            return View("Users", answer);
        }

        public IActionResult AllowanceReset()
        {
            if (!_cookieHelper.IsThereAGameMasterCookie())
            {
                return RedirectToAction("Login", "Home");
            }

            var answer = _repo.ResetAllowances();

            return View("Users", answer);
        }

        public IActionResult EnterUserName()
        {
            if (!_cookieHelper.IsThereAGameMasterCookie())
            {
                return RedirectToAction("Login", "Home");
            }

            var input = new EnterUserInputOutput
            {
                AllUsersLoginNames = _repo.GetAllUsers().Select(user => user.LoginName).ToList()
            };

            return View(input);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EnterUserName(EnterUserInputOutput output)
        {
            var answer = _repo.GetAllAccountsOfUser(output.UserLoginName);

            return View("UserBankAccounts", answer);
        }

        public IActionResult ChangeUserPassword()
        {
            if (!_cookieHelper.IsThereAGameMasterCookie())
            {
                return RedirectToAction("Login", "Home");
            }

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

        public IActionResult EditBankAccount(string accountNumber)
        {
            if (!_cookieHelper.IsThereAGameMasterCookie())
            {
                return RedirectToAction("Login", "Home");
            }

            var account = _repo.GetAccountByAccountNumber(accountNumber);

            var inputOutput = new InputOutputAccountEdit { Account = account, Errors = new List<string>()};

            return View( inputOutput );
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditBankAccount(InputOutputAccountEdit input)
        {
            var answer = _repo.EditBankAccount(input);

            return View("EditBankAccountResult", answer);
        }

        public IActionResult AddCompany()
        {
            if (!_cookieHelper.IsThereAGameMasterCookie())
            {
                return RedirectToAction("Login", "Home");
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddCompany(Company company)
        {
            var answer = _repo.AddCompany(company.Name);

            return View("AddCompanyResult", answer);
        }

        public IActionResult AddStocksForUser()
        {
            if (!_cookieHelper.IsThereAGameMasterCookie())
            {
                return RedirectToAction("Login", "Home");
            }

            var input = new AddStocksInputOutput
            {
                Companies = _stocksRepo.GetAllCompanies(),
                Users = _repo.GetAllUsers(),
                UserCompanyStocks = new UserCompanyStocks()
            };

            return View(input);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddStocksForUser(AddStocksInputOutput output)
        {
            output.Errors = new List<string>();

            var user = _repo.GetUserByLoginName(output.UserCompanyStocks.User.LoginName);

            var company = _stocksRepo.GetCompanyByName(output.UserCompanyStocks.Company.Name);

            if (user == null) output.Errors.Add($"No user was found with the login name \"{output.UserCompanyStocks.User.LoginName}\"");

            if (company == null) output.Errors.Add($"No company was found with the name \"{output.UserCompanyStocks.Company.Name}\"");

            if (output.NumberOfStocks == 0) output.Errors.Add($"A zero stocks change is pointless.");

            if(output.Errors.Count == 0)
            {
                output.UserCompanyStocks.Company = company;
                output.UserCompanyStocks.User = user;

                output.UserCompanyStocks.companyId = company.Id;
                output.UserCompanyStocks.userId = user.Id;

                output = _repo.AddStocksForUser(output);
            }

            return View("AddStocksForUserResult", output);
        }
    }
}
