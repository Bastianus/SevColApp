using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SevColApp.Repositories;

namespace SevColApp.Controllers
{
    public class GamemasterController : Controller
    {
        private readonly ILogger<GamemasterController> _logger;
        private IHomeRepository _userRepo;
        private IBankRepository _bankRepo;

        public GamemasterController(ILogger<GamemasterController> logger, IHomeRepository homeRepo, IBankRepository bankRepo)
        {
            _logger = logger;
            _userRepo = homeRepo;
            _bankRepo = bankRepo;
        }

        public IActionResult Index()
        {
            if (!IsThereAGameMasterCookie())
            {
                return RedirectToAction("Login", "Home");
            }

            return View();
        }

        private bool IsThereAGameMasterCookie()
        {
            if (Request.Cookies.ContainsKey("UserId")) 
            {
                return HttpContext.Request.Cookies["UserId"] == "7777777";
            };

            return false;
        }
    }
}
