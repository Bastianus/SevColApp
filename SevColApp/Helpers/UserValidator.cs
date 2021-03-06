using SevColApp.Models;
using System.Collections.Generic;

namespace SevColApp.Helpers
{
    public static class UserValidator
    {
        public static User ValidateUser(User user)
        {
            user.Errors = new List<string>();

            if (user.LoginName == null || user.LoginName.Length < 2) user.Errors.Add("Your login name should be at least 2 characters long.");
            else if (user.LoginName.Length > 30) user.Errors.Add("Your login name can be no longer than 30 characters.");

            if (user.FirstName == null || user.FirstName.Length < 2) user.Errors.Add("Your first name should be at least 2 characters long.");
            else if (user.FirstName.Length > 30) user.Errors.Add("Your first name can be no longer than 30 characters.");

            if (user.LastName == null || user.LastName.Length < 2) user.Errors.Add("Your last name should be at least 2 characters long.");
            else if (user.LastName.Length > 30) user.Errors.Add("Your last name can be no longer than 30 characters.");

            if (user.Prefixes != null && user.Prefixes.Length > 20) user.Errors.Add("Your prefixes can be no longer than 20 characters.");

            if (string.IsNullOrEmpty(user.Password)) user.Errors.Add("You need to set a password.");

            return user; 
        }
    }
}
