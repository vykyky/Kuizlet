using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Kuizlet.Application.Interfaces.Repositories;


namespace Kuizlet.Infrastructure.Implementations
{
    internal class PasswordHasher : IPasswordHasher
    {
        public (string Hash, string Salt) CreateHashWithSalt(string password)
        {
            string salt = GenerateSalt();
            string hash = ComputeHash(password, salt);
            return (hash, salt);
        }
        public string GenerateSalt()
        {
            byte[] saltBytes = new byte[16];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(saltBytes);
            }
            return Convert.ToBase64String(saltBytes);
        }

        public string ComputeHash(string password, string salt)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] passwordBytes = Encoding.UTF8.GetBytes(password + salt);
                byte[] hashBytes = sha256.ComputeHash(passwordBytes);
                return Convert.ToBase64String(hashBytes);
            }
        }

        public bool Verify(string password, string hash, string salt)
        {
            string computedHash = ComputeHash(password, salt);
            return computedHash == hash;
        }
    }
}
