using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Core.Common.Hashing
{
    public static class PasswordHashing
    {
        public static PasswordHashResult HashPassword(string password)
        {
            // generate a 128-bit salt using a secure PRNG
            byte[] salt = new byte[128 / 8];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            // derive a 256-bit subkey (use HMACSHA1 with 10,000 iterations)
            string hashedPasswordWithSalt = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                    password: password,
                    salt: salt,
                    prf: KeyDerivationPrf.HMACSHA1,
                    iterationCount: 10000,
                    numBytesRequested: 256 / 8));

            return new PasswordHashResult { Salt = Convert.ToBase64String(salt), Hash = hashedPasswordWithSalt };
        }

        public static bool ValidatePassword(string password, string storedHashWithSalt, string salt)
        {
            var saltBytes = Convert.FromBase64String(salt);

            string hashedPasswordWithSalt = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                    password: password,
                    salt: saltBytes,
                    prf: KeyDerivationPrf.HMACSHA1,
                    iterationCount: 10000,
                    numBytesRequested: 256 / 8));

            if (hashedPasswordWithSalt == storedHashWithSalt)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    public class PasswordHashResult
    {
        public string Salt { get; set; }
        public string Hash { get; set; }
    }
}

