using SevColApp.Context;
using SevColApp.Helpers;
using SevColApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SevColApp.Repositories
{
    public class GamemasterRepository : IGamemasterRepository
    {
        private readonly SevColContext _context;
        public GamemasterRepository(SevColContext context)
        {
            _context = context;
        }

        public User ChangeUserPassword(int userId, string newPassword)
        {
            var user = _context.Users.Find(userId);

            user.PasswordHash = PasswordHelper.GetPasswordHash(newPassword);

            _context.SaveChanges();

            return user;
        }
    }
}
