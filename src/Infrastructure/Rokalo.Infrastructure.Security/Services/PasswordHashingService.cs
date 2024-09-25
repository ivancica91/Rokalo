namespace Rokalo.Infrastructure.Security
{
    using Microsoft.AspNetCore.Cryptography.KeyDerivation;
    using Rokalo.Application.Contracts.Security;
    using System;
    using System.Security.Cryptography;

    internal sealed class PasswordHashingService : IPasswordHashingService
    {
        private static readonly RandomNumberGenerator Rng = RandomNumberGenerator.Create();

        private static readonly KeyDerivationPrf Pbkdf2Prf = KeyDerivationPrf.HMACSHA1;

        private static readonly int Pbkdf2IterCount = 1000;

        private static readonly int Pbkdf2SubkeyLength = 256 / 8;

        private static readonly int SaltSize = 128 / 8;

        public string Hash(string password)
        {
            var hash = HashPassword(password);

            return Convert.ToBase64String(hash);
        }

        public bool VerifyHash(string password, string providedPassword)
        {
            if (password is null || providedPassword is null)
            {
                return false;
            }

            return VerifyPasswordHash(password, providedPassword);
        }

        private static byte[] HashPassword(string password)
        {
            byte[] salt = new byte[SaltSize];

            Rng.GetBytes(salt);

            byte[] subkey = KeyDerivation.Pbkdf2(password, salt, Pbkdf2Prf, Pbkdf2IterCount, Pbkdf2SubkeyLength);

            var outputBytes = new byte[1 + SaltSize + Pbkdf2SubkeyLength];

            outputBytes[0] = 0x00;
            
            Buffer.BlockCopy(salt, 0, outputBytes, 1, SaltSize);

            Buffer.BlockCopy(subkey, 0, outputBytes, 1 + SaltSize, Pbkdf2SubkeyLength);

            return outputBytes;
        }

        private static bool VerifyPasswordHash(string password, string providedPassword)
        {
            byte[] hashedPassword = Convert.FromBase64String(password);

            if (hashedPassword is null || hashedPassword.Length == 0)
            {
                return false;
            }

            if (hashedPassword.Length != 1 + SaltSize + Pbkdf2SubkeyLength)
            {
                return false;
            }

            byte[] salt = new byte[SaltSize];

            Buffer.BlockCopy(hashedPassword, 1, salt, 0, salt.Length);

            byte[] expectedSubKey = new byte[Pbkdf2SubkeyLength];

            Buffer.BlockCopy(hashedPassword, 1 + salt.Length, expectedSubKey, 0, expectedSubKey.Length);

            // hash the provided password and verify 
            byte[] actualSubkey = KeyDerivation.Pbkdf2(providedPassword, salt, Pbkdf2Prf, Pbkdf2IterCount, Pbkdf2SubkeyLength);

            return ByteArraysEqual(actualSubkey, expectedSubKey);
        }

        private static bool ByteArraysEqual(byte[] a, byte[] b)
        {
            if (a == null && b == null)
            {
                return true;
            }

            if (a == null || b == null || a.Length != b.Length)
            {
                return false;
            }

            var areSame = true;

            for (var i = 0; i < a.Length; i++)
            {
                areSame &= (a[i] == b[i]);
            }

            return areSame;
        }
    }
}
