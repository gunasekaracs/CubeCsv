using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Globalization;
using System.IO;
using System.Text;

namespace CubeCsv.Tests
{
    [TestClass]
    public class CsvEncryptionTests
    {
        private readonly ASCIIEncoding encoding = new();

        [TestMethod]
        public void CsvCryptoHandler_EncryptDecryptValue_ShouldWorkCorrectly()
        {
            string originalValue = "Sensitive Data 2024";
            string key = "SecretKey!@#$%^&*()";
            
            CsvCryptoHandler handler = new();
            string encrypted = handler.EncryptValue(originalValue, key);
            string decrypted = handler.DecryptValue(encrypted, key);
            
            Assert.IsTrue(originalValue != encrypted);
            Assert.IsTrue(originalValue == decrypted);
        }

        [TestMethod]
        public void CsvCryptoHandler_EncryptValue_DifferentKeys_ShouldProduceDifferentResults()
        {
            string value = "Test Value";
            string key1 = "Key1";
            string key2 = "Key2";
            
            CsvCryptoHandler handler = new();
            string encrypted1 = handler.EncryptValue(value, key1);
            string encrypted2 = handler.EncryptValue(value, key2);
            
            Assert.IsTrue(encrypted1 != encrypted2);
        }

        [TestMethod]
        public void CsvCryptoHandler_DecryptValue_WithWrongKey_ShouldHandleGracefully()
        {
            string originalValue = "Secret Information";
            string correctKey = "CorrectKey123";
            string wrongKey = "WrongKey456";
            
            CsvCryptoHandler handler = new();
            string encrypted = handler.EncryptValue(originalValue, correctKey);
            
            try
            {
                string decryptedWithWrongKey = handler.DecryptValue(encrypted, wrongKey);
                Assert.IsTrue(originalValue != decryptedWithWrongKey);
            }
            catch (System.Security.Cryptography.CryptographicException)
            {
                // Wrong key may throw exception - this is expected
                Assert.IsTrue(true);
            }
        }
    }
}
