using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SevColApp.Helpers;
using SevColApp.Models;
using SevColApp.Repositories;

namespace SevColApp.Controllers
{
    public class GamemasterController : Controller
    {
        private readonly ILogger<GamemasterController> _logger;
        private readonly IGamemasterRepository _repo;
        private readonly IHomeRepository _userRepo;
        private readonly CookieHelper _cookieHelper;

        public GamemasterController(ILogger<GamemasterController> logger, IGamemasterRepository repo, IHomeRepository userRepo, CookieHelper cookieHelper)
        {
            _logger = logger;
            _repo = repo;
            _userRepo = userRepo;
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

            var userId = _userRepo.FindUserIdByLoginName(input.UserLoginName);

            _repo.ChangeUserPassword(userId, input.NewPassword);

            return View("UserPasswordChanged", input);
        }

        
    }
}
