using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace CubeCsv
{
    public class CsvCryptoHandler
    {
        #region Attributes

        private static readonly byte[] EncryptionSalt = new byte[] { 101, 176, 223, 18, 116, 37, 242, 92, 115, 50, 100, 42, 76, 132, 250, 2 };
        private static readonly int Rfc2898KeygenIterations = 100;
        private static readonly int AesKeySizeInBits = 128;

        #endregion

        #region Public Methods

        public string Encrypt(string input, string key)
        {
            byte[] rawBytes = Encoding.Unicode.GetBytes(input);
            byte[] rawCipherText = null;
            using (var aes = GetAes(key))
            {
                using (var ms = new MemoryStream())
                {
                    using (var cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(rawBytes, 0, rawBytes.Length);
                    }
                    rawCipherText = ms.ToArray();
                }
            }
            return Convert.ToBase64String(rawCipherText);
        }

        public string Decrypt(string input, string key)
        {
            byte[] rawCipherText = Convert.FromBase64String(input);
            byte[] rawPlainText = null;
            using (var aes = GetAes(key))
            {
                using (var ms = new MemoryStream())
                {
                    using (var cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(rawCipherText, 0, rawCipherText.Length);
                    }
                    rawPlainText = ms.ToArray();
                }
            }
            return Encoding.Unicode.GetString(rawPlainText);
        }

        #endregion

        #region Private Methods

        private static Aes GetAes(string key)
        {
            var aes = new AesManaged();
            aes.Padding = PaddingMode.PKCS7;
            aes.KeySize = AesKeySizeInBits;

            int KeyStrengthInBytes = aes.KeySize / 8;
            var rfc2898 = new Rfc2898DeriveBytes(key, EncryptionSalt, Rfc2898KeygenIterations);
            aes.Key = rfc2898.GetBytes(KeyStrengthInBytes);
            aes.IV = rfc2898.GetBytes(KeyStrengthInBytes);

            return aes;
        }

        #endregion
    }
}
