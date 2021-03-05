using Microsoft.AspNetCore.Http;
using System;

namespace SevColApp.Helpers
{
    public class CookieHelper
    {
        private IHttpContextAccessor _accessor;
        public CookieHelper(IHttpContextAccessor accessor)
        {
            _accessor = accessor;
        }
        public bool IsThereACookie()
        {
            return _accessor.HttpContext.Request.Cookies.ContainsKey("UserId");
        }

        public int GetUserIdFromCookie()
        {
            return Int32.Parse(_accessor.HttpContext.Request.Cookies["UserId"]);
        }
    }
}
