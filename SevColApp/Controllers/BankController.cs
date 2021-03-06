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
    public class BankController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHomeRepository _userRepo;
        private readonly IBankRepository _repo;
        private readonly CookieHelper _cookieHelper;

        public BankController(ILogger<HomeController> logger, IHomeRepository userRepo, IBankRepository repo, CookieHelper cookieHelper)
        {
            _logger = logger;
            _userRepo = userRepo;
            _repo = repo;
            _cookieHelper = cookieHelper;
        }

        public IActionResult Index()
        {
            if (!_cookieHelper.IsThereACookie())
            {
                return RedirectToAction("Login", "Home");
            }

            var id = _cookieHelper.GetUserIdFromCookie();

            var viewInput = new UserBankAccounts
            {
                User = _userRepo.FindUserById(id),

                BankAccounts = _repo.GetBankAccountsOfUser(id)
            };

            return View("Index", viewInput);
        }

        public IActionResult PasswordFill(int accountId)
        {
            if (!_cookieHelper.IsThereACookie())
            {
                return RedirectToAction("Login", "Home");
            }

            var account = _repo.GetBankAccountById(accountId);

            return View("PasswordFill", account);
        }

        public IActionResult PasswordCheck(BankAccount passwordData)
        {
            if (!ModelState.IsValid)
            {
                RedirectToAction("Privacy", "Home", null);
            }

            if (!_cookieHelper.IsThereACookie())
            {
                return RedirectToAction("Login", "Home");
            }

            var account = _repo.GetBankAccountById(passwordData.Id);

            var data = new BankAccountDetails() { Id = account.Id, AccountName = account.AccountName, AccountNumber = account.AccountNumber, Credit = account.Credit };

            var passwordIsCorrect = _repo.IsAccountPasswordCorrect(account.AccountNumber, passwordData.Password);

            if (passwordIsCorrect)
            {
                return RedirectToAction("Details", data);
            }

            return View();
        }

        public IActionResult Details(BankAccountDetails data)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Privacy", "Home");
            }

            return View("Details", data);
        }

        public IActionResult DetailsViaName(string accountName)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Privacy", "Home");
            }

            var data = _repo.GetBankAccountDetailsByAccountName(accountName);

            return View("Details", data);
        }

        public IActionResult Create()
        {
            if (!_cookieHelper.IsThereACookie())
            {
                return RedirectToAction("Login", "Home");
            }

            var userId = _cookieHelper.GetUserIdFromCookie();

            var input = new InputOutputAccountCreate
            {
                UserHasAccount = (_repo.GetBankAccountsOfUser(userId)).Any(),

                Banks = _repo.GetAllBanks()
            };

            return View("Create", input);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(InputOutputAccountCreate input) 
        {
            input.Errors = new List<string>();

            var possibleBankNames = _repo.GetAllBankNames();

            input = BankValidator.ValidateAccountInput(input, possibleBankNames);        

            if (!_cookieHelper.IsThereACookie())
            {
                return RedirectToAction("Login", "Home");
            }

            if(input.Errors.Count == 0)
            {
                var userId = _cookieHelper.GetUserIdFromCookie();

                _repo.CreateNewAccount(input, userId);
            }            

            return View("AccountCreated",input);
        }

        public IActionResult Transfer(int accountId)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Privacy", "Home");
            }

            var data = new InputOutputTransfer
            {
                BankAccount = _repo.GetBankAccountById(accountId)
            };

            return View("Transfer", data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Transfer(InputOutputTransfer input)
        {
            var transfer = input.Transfer;

            transfer.Errors = new List<string>();

            var possibleAccounts = _repo.GetAllBankAccountNumbers();

            transfer = BankValidator.ValidateTransfer(input.Transfer, possibleAccounts);

            if (transfer.Errors.Count == 0)
            {
                var passwordIsCorrect = _repo.IsAccountPasswordCorrect(input.Transfer.PayingAccountNumber, input.Password);

                if (passwordIsCorrect)
                {
                    transfer = _repo.ExecuteTransfer(transfer);
                }
                else
                {
                    transfer.Errors.Add("The account password was incorrect");
                }
                
            }

            return View("TransferResult", transfer);
        }
    }
}
