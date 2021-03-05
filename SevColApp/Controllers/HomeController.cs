using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SevColApp.Helpers;
using SevColApp.Models;
using SevColApp.Repositories;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace SevColApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHomeRepository _repo;
        private readonly CookieHelper _cookieHelper;

        public HomeController(ILogger<HomeController> logger, IHomeRepository repo, CookieHelper cookieHelper)
        {
            _logger = logger;
            _repo = repo;
            _cookieHelper = cookieHelper;
        }

        public IActionResult Create()
        {
            return View();
        }

        public IActionResult Login()
        {
            if (_cookieHelper.IsThereACookie())
            {
                return RedirectToAction("LoggedIn");
            }

            return View();
        }

        public IActionResult LoginUnknown()
        {
            return View();
        }

        public async Task<IActionResult> LoggedIn()
        {
            if (!_cookieHelper.IsThereACookie())
            {
                return RedirectToAction("Login");
            }

            var id = _cookieHelper.GetUserIdFromCookie();

            var user = await _repo.FindUserById(id);

            return View("LoggedIn", user);
        }

        public IActionResult Logout()
        {
            if (_cookieHelper.IsThereACookie())
            {
                _cookieHelper.RemoveCookie();
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(User user)
        {
            if (string.IsNullOrEmpty(user.LoginName) || string.IsNullOrEmpty(user.Password))
            {
                return RedirectToAction("Privacy");
            }

            if (_repo.LoginIsCorrect(user))
            {
                var userId = _repo.FindUserIdByLoginName(user.LoginName);

                _cookieHelper.MakeACookie(userId);

                return RedirectToAction(nameof(LoggedIn));
            }

            //return BadRequest("Login name unknown or password is incorrect");
            return RedirectToAction(nameof(LoginUnknown));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(User user)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Privacy");
            }

            var userId = await _repo.AddUserIfHeDoesNotExits(user);

            if(_repo.IsPasswordCorrect(user.Password, userId))
            {
                _cookieHelper.MakeACookie(userId);

                return RedirectToAction(nameof(LoggedIn));
            }

            return BadRequest("User already exists and/or password was incorrect");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult DeleteUser()
        {
            if (!_cookieHelper.IsThereACookie())
            {
                return RedirectToAction("Login");
            }

            var id = _cookieHelper.GetUserIdFromCookie();

            _repo.DeleteUserById(id);

            _cookieHelper.RemoveCookie();

            return RedirectToAction("Login");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
