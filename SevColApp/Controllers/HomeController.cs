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
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private IHomeRepository _repo;

        public HomeController(ILogger<HomeController> logger, IHomeRepository repo)
        {
            _logger = logger;
            _repo = repo;
        }

        public IActionResult Create()
        {
            return View();
        }

        public IActionResult Login()
        {
            if (IsThereACookie())
            {
                return RedirectToAction("LoggedIn");
            }

            return View();
        }

        public IActionResult LoginUnknown()
        {
            if (IsThereACookie())
            {
                return RedirectToAction("LoggedIn");
            }

            return View();
        }

        public async Task<IActionResult> LoggedIn()
        {
            if (!IsThereACookie())
            {
                return RedirectToAction("Login");
            }

            var id = GetUserIdFromCookie();

            var user = await _repo.FindUserById(id);

            return View("LoggedIn",user);
        }

        public IActionResult Logout()
        {
            if (IsThereACookie()) RemoveCookie();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(User user)
        {
            if (_repo.LoginIsCorrect(user))
            {
                var userId = _repo.FindUserIdByLoginName(user.LoginName);
                MakeACookie(userId);

                return RedirectToAction(nameof(LoggedIn));
            }

            //return BadRequest("Login name unknown or password is incorrect");
            return RedirectToAction(nameof(LoginUnknown));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(User user)
        {
            var userId = await _repo.AddUserIfHeDoesNotExits(user);

            if(_repo.IsPasswordCorrect(user.Password, userId))
            {
                MakeACookie(userId);

                return RedirectToAction(nameof(LoggedIn));
            }

            return BadRequest("Password was incorrect");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private void MakeACookie(int userId)
        {
            CookieOptions option = new CookieOptions();

            option.Expires = DateTime.Now.AddDays(1);

            Response.Cookies.Append("UserId", userId.ToString(), option);
        }

        private bool IsThereACookie()
        {
            return Request.Cookies.ContainsKey("UserId");
        }
        
        private int GetUserIdFromCookie()
        {
            return Int32.Parse(HttpContext.Request.Cookies["UserId"]);
        }

        private void RemoveCookie()
        {
            Response.Cookies.Delete("UserId");
        }
    }
}
