using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace FinalProject_API.Common
{
    public static class Crypto
    {
        public static string MD5(string input)
        {
            using var hash = System.Security.Cryptography.MD5.Create();
            var data = hash.ComputeHash(Encoding.UTF8.GetBytes(input));
            var builder = new StringBuilder();
            for (var i = 0; i < data.Length; i++)
                builder.Append(data[i].ToString("x2"));
            return builder.ToString();
        }

        /// <summary>
        /// sha256(123456) => 8d969eef6ecad3c29a3a629280e686cf0c3f5d5a86aff3ca12020c923adc6c92
        /// </summary>
        public static string SHA256(string input)
        {
            using var hash = System.Security.Cryptography.SHA256.Create();
            var data = hash.ComputeHash(Encoding.UTF8.GetBytes(input));
            var builder = new StringBuilder();
            for (var i = 0; i < data.Length; i++)
                builder.Append(data[i].ToString("x2"));
            return builder.ToString();
        }

        public static string CreateSalt()
        {
            var bouncyCastleHashing = new BouncyCastleHashing();
            return Convert.ToBase64String(bouncyCastleHashing.CreateSalt(16));
        }

        public static string Hash(string password, string salt)
        {
            var bouncyCastleHashing = new BouncyCastleHashing();
            return bouncyCastleHashing.PBKDF2_SHA256_GetHash(password, salt, 29000, 43);
        }

        public static bool Verify(string password, string salt, string hash)
        {
            var bouncyCastleHashing = new BouncyCastleHashing();
            return bouncyCastleHashing.ValidatePassword(password, Convert.FromBase64String(salt), 29000, 43, Convert.FromBase64String(hash));
        }

        /// <summary>
        /// Using for Email and Public (non-critical services)
        /// BCrypt: Iterations=8192, HashBytes=16
        /// </summary>
        public static string SimpleHash(string password, string salt)
        {
            var bouncyCastleHashing = new BouncyCastleHashing();
            return bouncyCastleHashing.PBKDF2_SHA256_GetHash(password, salt, 8192, 16);
        }

        /// <summary>
        /// Using for Email and Public (non-critical services)
        /// BCrypt: Iterations=8192, HashBytes=16
        /// </summary>
        public static bool SimpleVerify(string password, string salt, string hash)
        {
            var bouncyCastleHashing = new BouncyCastleHashing();
            return bouncyCastleHashing.ValidatePassword(password, Convert.FromBase64String(salt), 8192, 16, Convert.FromBase64String(hash));
        }

        public static long GetHashCode(string text)
        {
            if (text == null)
                throw new ArgumentNullException(nameof(text));

            if (text.Length <= 0)
                return 0;

            //Unicode Encode Covering all characterset
            var byteContents = Encoding.Unicode.GetBytes(text);
            using var hash = System.Security.Cryptography.SHA256.Create();
            var hashText = hash.ComputeHash(byteContents);
            //32Byte hashText separate
            //hashCodeStart = 0~7  8Byte
            //hashCodeMedium = 8~23  8Byte
            //hashCodeEnd = 24~31  8Byte
            //and Fold
            var hashCodeStart = BitConverter.ToInt64(hashText, 0);
            var hashCodeMedium = BitConverter.ToInt64(hashText, 8);
            var hashCodeEnd = BitConverter.ToInt64(hashText, 24);
            return hashCodeStart ^ hashCodeMedium ^ hashCodeEnd;
        }

        public static ulong GetHashCode2(string text)
        {
            if (text == null)
                throw new ArgumentNullException(nameof(text));

            if (text.Length <= 0)
                return 0;

            //Unicode Encode Covering all characterset
            var byteContents = Encoding.Unicode.GetBytes(text);
            using var hash = System.Security.Cryptography.SHA256.Create();
            var hashText = hash.ComputeHash(byteContents);
            //32Byte hashText separate
            //hashCodeStart = 0~7  8Byte
            //hashCodeMedium = 8~23  8Byte
            //hashCodeEnd = 24~31  8Byte
            //and Fold
            var hashCodeStart = BitConverter.ToUInt64(hashText, 0);
            var hashCodeMedium = BitConverter.ToUInt64(hashText, 8);
            var hashCodeEnd = BitConverter.ToUInt64(hashText, 24);
            return hashCodeStart ^ hashCodeMedium ^ hashCodeEnd;
        }
    }
}
