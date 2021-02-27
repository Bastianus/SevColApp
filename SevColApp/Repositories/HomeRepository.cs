using Microsoft.EntityFrameworkCore;
using SevColApp.Models;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SevColApp.Repositories
{
    public class HomeRepository : IHomeRepository
    {
        private SevColContext _context;
        public HomeRepository(SevColContext context)
        {
            _context = context;
        }

        public async Task<int> AddUserIfHeDoesNotExits(User user)
        {
            if (!_context.Users.Any(x => x.LoginName == user.LoginName))
            {
                user.PasswordHash = GetPasswordHash(user.Password);

                _context.Users.Add(user);

                await _context.SaveChangesAsync();
            }

            return _context.Users.Single(x => x.LoginName == user.LoginName).Id;
        }

        public async Task<User> FindUserById(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        private byte[] GetPasswordHash(string password)
        {
            if (string.IsNullOrEmpty(password)) return new byte[] { };

            using (HashAlgorithm algorithm = SHA512.Create())
            {
                return algorithm.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }

        public bool IsPasswordCorrect(string password, int userId)
        {
            var givenPasswordHash = GetPasswordHash(password);

            var PasswordhashOfSavedUser = _context.Users.Find(userId).PasswordHash;

            return givenPasswordHash.SequenceEqual(PasswordhashOfSavedUser);
        }

        public bool LoginIsCorrect(User user)
        {
            var databaseUser = _context.Users.Where(x => x.LoginName == user.LoginName).FirstOrDefault();

            if (databaseUser == null) return false;

            if (IsPasswordCorrect(user.Password, databaseUser.Id)) return true;

            return false;            
        }

        public int FindUserIdByLoginName(string name)
        {
            var userId = _context.Users.Where(x => x.LoginName == name).First().Id;
            return userId;
        }
    }
}
