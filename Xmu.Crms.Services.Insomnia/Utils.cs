using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace Xmu.Crms.Services.Insomnia
{
    public static class PasswordUtils
    {
        public static byte[] GenerateSalt(int saltSize = 3)
        {
            using (var rng = new RNGCryptoServiceProvider())
            {
                var randomNumber = new byte[saltSize];
                rng.GetBytes(randomNumber);
                return randomNumber;
            }
        }

        public static Tuple<byte[], byte[]> ReadHashString(string hash)
        {
            var salts = Convert.FromBase64String(hash.Substring(0, 4));
            var hashs = Convert.FromBase64String(hash.Substring(4));
            return Tuple.Create(salts, hashs);
        }

        public static string HashString(string password)
        {
            var salt = GenerateSalt();
            return Convert.ToBase64String(salt) + Convert.ToBase64String(Hash(password, salt));
        }

        public static string HashString(string password, byte[] salt) =>
            Convert.ToBase64String(salt) + Convert.ToBase64String(Hash(password, salt));


        public static byte[] Hash(string password, byte[] salt) =>
            KeyDerivation.Pbkdf2(
                password,
                salt,
                KeyDerivationPrf.HMACSHA256,
                10000,
                9);

        public static bool IsExpectedPassword(string password, byte[] salt, byte[] expectedHash) =>
            ConstantTimeEquals(Hash(password, salt), expectedHash);

        internal static bool IsExpectedPassword(string password, Tuple<byte[], byte[]> tuple) =>
            IsExpectedPassword(password, tuple.Item1, tuple.Item2);

        private static bool ConstantTimeEquals(IReadOnlyList<byte> a, IReadOnlyList<byte> b)
        {
            var diff = (uint) a.Count ^ (uint) b.Count;
            for (var i = 0; i < a.Count && i < b.Count; i++)
            {
                diff |= (uint) (a[i] ^ b[i]);
            }

            return diff == 0;
        }
    }
}