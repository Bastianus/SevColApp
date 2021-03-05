using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace SevColApp.Helpers
{
    public static class PasswordHelper
    {
        public static byte[] GetPasswordHash(string password)
        {
            if (string.IsNullOrEmpty(password)) return Array.Empty<byte>();

            using HashAlgorithm algorithm = SHA512.Create();
            return algorithm.ComputeHash(Encoding.UTF8.GetBytes(password));
        }

        public static bool PasswordCheck(byte[] inputPasswordHash, byte[] originalPasswordHash)
        {
            return inputPasswordHash.SequenceEqual(originalPasswordHash);
        }

        public static bool PasswordCheck(string password, byte[] originalPasswordHash)
        {
            byte[] inputPasswordHash = GetPasswordHash(password);

            return PasswordCheck(inputPasswordHash, originalPasswordHash);
        }
    }
}
