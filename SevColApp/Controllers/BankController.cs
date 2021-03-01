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
            if (!IsThereACookie())
            {
                return RedirectToAction("Login", "Home");
            }

            var account = await _repo.GetBankAccountById(passwordData.Id);

            var passwordHash = _userRepo.GetPasswordHash(passwordData.Password);

            if(account.PasswordHash.SequenceEqual(passwordHash))
            {
                return RedirectToAction("Details", account);
            }

            return View();
        }

        public  IActionResult Details(BankAccount account)
        {
            if (!IsThereACookie())
            {
                return RedirectToAction("Login", "Home");
            }

            return View("Details", account);
        }

        public IActionResult Create()
        {
            if (!IsThereACookie())
            {
                return RedirectToAction("Login", "Home");
            }

            return View();
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
