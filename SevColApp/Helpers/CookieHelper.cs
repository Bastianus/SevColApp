using Microsoft.AspNetCore.Http;
using SevColApp.Repositories;
using System;

namespace SevColApp.Helpers
{
    public class CookieHelper
    {
        private readonly IHttpContextAccessor _context;
        private readonly string _gameMasterId;
        public CookieHelper(IHttpContextAccessor accessor, IHomeRepository userRepo)
        {
            _context = accessor;
            _gameMasterId = userRepo.FindUserIdByLoginName("GameMaster").ToString();
        }
        public bool IsThereACookie()
        {
            return _context.HttpContext.Request.Cookies.ContainsKey("UserId");
        }

        public int GetUserIdFromCookie()
        {
            return Int32.Parse(_context.HttpContext.Request.Cookies["UserId"]);
        }
        public void RemoveCookie()
        {
            _context.HttpContext.Response.Cookies.Delete("UserId");
        }

        public void MakeACookie(int userId)
        {
            var option = new CookieOptions
            {
                Expires = DateTime.Now.AddDays(1)
            };

            _context.HttpContext.Response.Cookies.Append("UserId", userId.ToString(), option);
        }

        public bool IsThereAGameMasterCookie()
        {
            if (_context.HttpContext.Request.Cookies.ContainsKey("UserId"))
            {
                return _context.HttpContext.Request.Cookies["UserId"] == _gameMasterId;
            };

            return false;
        }

        public int GetGameMasterId()
        {
            return Int32.Parse(_gameMasterId);
        }
    }
}
