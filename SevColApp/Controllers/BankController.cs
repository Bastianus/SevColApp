using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SevColApp.Models;
using SevColApp.Repositories;
using System;
using System.Diagnostics;
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

        public async Task<IActionResult> Details(int bankId)
        {
            if (!IsThereACookie())
            {
                return RedirectToAction("Login", "Home");
            }

            return View();
        }

        public IActionResult PasswordCheck()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult PasswordCheck(BankAccount account)
        {
            return RedirectToAction("Details");
        }

        public async Task<IActionResult> Create()
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
