using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using SevColApp.Helpers;
using SevColApp.Models;
using SevColApp.Repositories;
using System;
using System.Linq;
using System.Threading.Tasks;

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

        public async Task<IActionResult> Index()
        {
            if (!_cookieHelper.IsThereACookie())
            {
                return RedirectToAction("Login", "Home");
            }

            var id = _cookieHelper.GetUserIdFromCookie();

            var viewInput = new UserBankAccounts();

            viewInput.User = await _userRepo.FindUserById(id);

            viewInput.BankAccounts = await _repo.GetBankAccountsOfUser(id);

            return View("Index", viewInput);
        }

        public async Task<IActionResult> PasswordFill(int accountId)
        {
            if (!_cookieHelper.IsThereACookie())
            {
                return RedirectToAction("Login", "Home");
            }

            var account = await _repo.GetBankAccountById(accountId);

            return View("PasswordFill", account);
        }

        public async Task<IActionResult> PasswordCheck(BankAccount passwordData)
        {
            if (!ModelState.IsValid)
            {
                RedirectToAction("Privacy", "Home", null);
            }

            if (!_cookieHelper.IsThereACookie())
            {
                return RedirectToAction("Login", "Home");
            }

            var account = await _repo.GetBankAccountById(passwordData.Id);

            var data = new BankAccountDetails() { Id = account.Id, AccountName = account.AccountName, AccountNumber = account.AccountNumber, Credit = account.Credit };

            var passwordIsCorrect = await _repo.IsAccountPasswordCorrect(account.AccountNumber, passwordData.Password);

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

        public async Task<IActionResult> Create()
        {
            if (!_cookieHelper.IsThereACookie())
            {
                return RedirectToAction("Login", "Home");
            }

            var userId = _cookieHelper.GetUserIdFromCookie();

            var input = new InputOutputAccountCreate();

            input.UserHasAccount = (await _repo.GetBankAccountsOfUser(userId)).Any();

            input.Banks = await _repo.GetAllBanks();

            return View("Create", input);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(InputOutputAccountCreate input)
        {
            if (!ModelState.IsValid)
            {
                RedirectToAction("Privacy", "Home");
            }

            if (!_cookieHelper.IsThereACookie())
            {
                return RedirectToAction("Login", "Home");
            }

            var userId = _cookieHelper.GetUserIdFromCookie();

            _repo.CreateNewAccount(input, userId);

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Transfer(int accountId)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Privacy", "Home");
            }

            var data = new InputOutputTransfer();

            data.BankAccount = await _repo.GetBankAccountById(accountId);

            return View("Transfer", data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Transfer(InputOutputTransfer input)
        {
            var transfer = input.Transfer;

            if (ModelState.IsValid)
            {
                var passwordIsCorrect = await _repo.IsAccountPasswordCorrect(input.Transfer.PayingAccountNumber, input.Password);

                if (passwordIsCorrect)
                {
                    transfer = _repo.ExecuteTransfer(transfer);
                }
                else
                {
                    transfer.Error = "The account password was incorrect";
                }
                
            }
            else
            {
                RedirectToAction("Privacy", "Home");
            }

            return View("TransferResult", transfer);
        }
    }
}
