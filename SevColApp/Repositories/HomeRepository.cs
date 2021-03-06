using SevColApp.Context;
using SevColApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace SevColApp.Repositories
{
    public class HomeRepository : IHomeRepository
    {
        private readonly SevColContext _context;
        public HomeRepository(SevColContext context)
        {
            _context = context;
        }

        public int AddUserIfHeDoesNotExits(User user)
        {
            if (!_context.Users.Any(x => x.LoginName == user.LoginName))
            {
                user.PasswordHash = GetPasswordHash(user.Password);

                _context.Users.Add(user);

                _context.SaveChanges();
            }

            return _context.Users.Single(x => x.LoginName == user.LoginName).Id;
        }

        public User FindUserById(int id)
        {
            return _context.Users.Find(id);
        }

        private static byte[] GetPasswordHash(string password)
        {
            if (string.IsNullOrEmpty(password)) return Array.Empty<byte>();

            using HashAlgorithm algorithm = SHA512.Create();
            return algorithm.ComputeHash(Encoding.UTF8.GetBytes(password));
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

        public List<User> GetAllOtherUsers(int id)
        {
            var allUsers = _context.Users.ToList();
            return allUsers.Where(x => x.Id != id).ToList();
        }

        public void DeleteUserById(int id)
        {
            var user = _context.Users.Find(id);

            _context.Users.Remove(user);

            _context.SaveChanges();
        }
    }
}
