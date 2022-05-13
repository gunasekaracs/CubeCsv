using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace CubeCsv
{
    class CsvCryptoHandler
    {
        private static readonly byte[] EncryptionSalt = new byte[] { 10, 176, 223, 6, 116, 75, 242, 92, 15, 150, 100, 223, 76, 132, 250, 251 };
        private static readonly int Rfc2898KeygenIterations = 100;
        private static readonly int AesKeySizeInBits = 128;

        public string Encrypt(string value, string key)
        {
            byte[] bytes = Encoding.Unicode.GetBytes(value);
            byte[] encrypted;
            using (var aes = GetAes(key))
            using (MemoryStream memoryStream = new MemoryStream())
            using (var cryptoStream = new CryptoStream(memoryStream, aes.CreateEncryptor(), CryptoStreamMode.Write))
            {
                cryptoStream.Write(bytes, 0, bytes.Length);
                encrypted = memoryStream.ToArray();
                return Convert.ToBase64String(encrypted);
            }
        }
        public string Decrypt(string value, string key)
        {
            byte[] bytes = Convert.FromBase64String(value);
            byte[] decrypted;
            using (var aes = GetAes(key))
            using (var memoryStream = new MemoryStream())
            using (var cryptoStream = new CryptoStream(memoryStream, aes.CreateDecryptor(), CryptoStreamMode.Write))
            {
                cryptoStream.Write(bytes, 0, bytes.Length);
                decrypted = memoryStream.ToArray();
                return Encoding.Unicode.GetString(decrypted);
            }
        }

        private static Aes GetAes(string key, byte[] keyOveride = null)
        {
            var aes = Aes.Create();
            aes.Padding = PaddingMode.PKCS7;
            aes.KeySize = AesKeySizeInBits;
            if (keyOveride == null)
            {
                int KeyStrengthInBytes = aes.KeySize / 8;
                var rfc2898 = new Rfc2898DeriveBytes(key, EncryptionSalt, Rfc2898KeygenIterations);
                aes.Key = rfc2898.GetBytes(KeyStrengthInBytes);
                aes.IV = rfc2898.GetBytes(KeyStrengthInBytes);
            }
            else
            {
                aes.Key = keyOveride;
                aes.IV = keyOveride;
            }
            return aes;
        }
    }
}
