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
            if (!_context.Users.Any(x => x.FirstName == user.FirstName && x.LastName == user.LastName))
            {
                user.PasswordHash = GetPasswordHash(user.Password);

                _context.Users.Add(user);

                await _context.SaveChangesAsync();
            }

            return _context.Users.Single(x => x.FirstName == user.FirstName && x.LastName == user.LastName).Id;
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
    }
}
