using Microsoft.AspNetCore.Http;
using System;

namespace SevColApp.Helpers
{
    public class CookieHelper
    {
        private readonly IHttpContextAccessor _context;
        public CookieHelper(IHttpContextAccessor accessor)
        {
            _context = accessor;
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
    }
}
