using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
        private IHomeRepository _userRepo;
        private IBankRepository _repo;

        public BankController(ILogger<HomeController> logger, IHomeRepository userRepo, IBankRepository repo)
        {
            _logger = logger;
            _userRepo = userRepo;
            _repo = repo;
        }

        public async Task<IActionResult> Index()
        {
            if (!IsThereACookie())
            {
                return RedirectToAction("Login", "Home");
            }

            var id = GetUserIdFromCookie();

            var viewInput = new UserBankAccounts();

            viewInput.User = await _userRepo.FindUserById(id);

            viewInput.BankAccounts = await _repo.GetBankAccountsOfUser(id);

            return View("Index", viewInput);
        }

        public async Task<IActionResult> PasswordFill(int accountId)
        {
            if (!IsThereACookie())
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

            if (!IsThereACookie())
            {
                return RedirectToAction("Login", "Home");
            }

            var account = await _repo.GetBankAccountById(passwordData.Id);

            var passwordIsCorrect = await _repo.IsAccountPasswordCorrect(account.AccountNumber, passwordData.Password);

            if (passwordIsCorrect)
            {
                return RedirectToAction("Details", account);
            }

            return View();
        }

        public IActionResult Details(BankAccount account)
        {
            if (!IsThereACookie())
            {
                return RedirectToAction("Login", "Home");
            }

            return View("Details", account);
        }

        public async Task<IActionResult> Create()
        {
            if (!IsThereACookie())
            {
                return RedirectToAction("Login", "Home");
            }
            var userId = GetUserIdFromCookie();

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
                RedirectToAction("Privacy", "Home", null);
            }

            var userId = GetUserIdFromCookie();

            _repo.CreateNewAccount(input, userId);

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Transfer(int accountId)
        {
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
                RedirectToAction("Privacy", "Home", null);
            }

            return View("TransferResult", transfer);
        }

        private bool IsThereACookie()
        {
            return Request.Cookies.ContainsKey("UserId");
        }

        private int GetUserIdFromCookie()
        {
            return Int32.Parse(HttpContext.Request.Cookies["UserId"]);
        }
    }
}
