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

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> LoggedIn()
        {
            var id = GetUserIdFromCookie();

            var user = await _repo.FindUserById(id);

            return View("LoggedIn",user);
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
        
        private int GetUserIdFromCookie()
        {
            return Int32.Parse(HttpContext.Request.Cookies["UserId"]);
        }
    }
}
