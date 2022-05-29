using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

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

        public string EncryptValue(string input, string key)
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

        public string DecryptValue(string input, string key)
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

        public async Task<TableDirect> ConvertCrypto(bool encrypt, TableDirect tableDirect, string key, CsvHeader header, CsvConfiguration configuration, string[] columnExclusions = null)
        {
            MemoryStream stream = new MemoryStream();
            StreamReader reader = new StreamReader(stream);
            StreamWriter writer = new StreamWriter(stream);
            while (await tableDirect.ReadAsync())
            {
                if(encrypt) tableDirect.Current.Encrypt(key, columnExclusions, this, header);
                else tableDirect.Current.Decrypt(key, columnExclusions, this, header);
                writer.WriteLine(tableDirect.Current.ToString(configuration.Delimiter));
            }
            writer.Flush();
            tableDirect.Dispose();
            return new TableDirect(reader, configuration);
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
