using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CubeCsv.Tests
{
    [TestClass]
    public class CsvCryptoTests
    {
        [TestMethod]
        public void CsvCryptoTest()
        {
            string value = "Intelligent Client Lifecycle Management";
            string key = "The bytes in this string are used to derive an encryption key, so do not change it or you will break the encryption/decryption :)";
            CsvCryptoHandler handler = new();
            string encrypted = handler.EncryptValue(value, key);
            string decrypted = handler.DecryptValue(encrypted, key);
            Assert.IsTrue(string.Compare(value, decrypted) == 0);
        }
    }
}
