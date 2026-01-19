using CubeCsv.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace CubeCsv.Tests
{
    [TestClass]
    public class CsvRowTests
    {
        private readonly ASCIIEncoding encoding = new();

        [TestMethod]
        public void CsvRow_AddHashColumn_ShouldAddSHA256Hash()
        {
            var data = encoding.GetBytes(CsvFiles.GeneralCsv);
            using var stream = new MemoryStream(data);
            using var streamReader = new StreamReader(stream);
            using var csvFile = new CsvFile(streamReader, new CsvConfiguration() { HasHeader = true, CultureInfo = CultureInfo.InvariantCulture });
            
            csvFile.ReadAsync().Wait();
            var row = csvFile.Current;
            int originalCount = row.Count;
            
            row.AddHashColumn();
            
            Assert.IsTrue(row.Count == originalCount + 1);
            Assert.IsTrue(row[row.Count - 1].Value != null);
            Assert.IsTrue(row[row.Count - 1].Value.ToString().Length == 64); // SHA256 hash length
        }

        [TestMethod]
        public void CsvRow_ToJson_ShouldConvertToJsonFormat()
        {
            var data = encoding.GetBytes(CsvFiles.GeneralCsv);
            using var stream = new MemoryStream(data);
            using var streamReader = new StreamReader(stream);
            using var csvFile = new CsvFile(streamReader, new CsvConfiguration() { HasHeader = true, CultureInfo = CultureInfo.InvariantCulture });
            
            csvFile.ReadAsync().Wait();
            var row = csvFile.Current;
            string json = row.ToJson(csvFile.Header);
            
            Assert.IsTrue(json.StartsWith("{"));
            Assert.IsTrue(json.EndsWith("}"));
            Assert.IsTrue(json.Contains("\"Department\""));
            Assert.IsTrue(json.Contains("\"First Name\""));
        }

        [TestMethod]
        public void CsvRow_SetValue_ShouldUpdateFieldValue()
        {
            var data = encoding.GetBytes(CsvFiles.GeneralCsv);
            using var stream = new MemoryStream(data);
            using var streamReader = new StreamReader(stream);
            using var csvFile = new CsvFile(streamReader, new CsvConfiguration() { HasHeader = true, CultureInfo = CultureInfo.InvariantCulture });
            
            csvFile.ReadAsync().Wait();
            csvFile.SetValue(HeaderNames.Department, "NEW_DEPT");
            
            Assert.IsTrue(csvFile.GetValue<string>(HeaderNames.Department) == "NEW_DEPT");
        }

        [TestMethod]
        public void CsvRow_SetValue_WithNullValue_ShouldThrowException()
        {
            var data = encoding.GetBytes(CsvFiles.GeneralCsv);
            using var stream = new MemoryStream(data);
            using var streamReader = new StreamReader(stream);
            using var csvFile = new CsvFile(streamReader, new CsvConfiguration() { HasHeader = true, CultureInfo = CultureInfo.InvariantCulture });
            
            csvFile.ReadAsync().Wait();
            
            Assert.ThrowsException<CsvNullValueException>(() => csvFile.SetValue(0, null));
        }

        [TestMethod]
        public void CsvRow_SetValue_WithOutOfBoundsLocation_ShouldThrowException()
        {
            var data = encoding.GetBytes(CsvFiles.GeneralCsv);
            using var stream = new MemoryStream(data);
            using var streamReader = new StreamReader(stream);
            using var csvFile = new CsvFile(streamReader, new CsvConfiguration() { HasHeader = true, CultureInfo = CultureInfo.InvariantCulture });
            
            csvFile.ReadAsync().Wait();
            
            Assert.ThrowsException<CsvOutBoundException>(() => csvFile.SetValue(999, "value"));
        }

        [TestMethod]
        public void CsvRow_ToString_ShouldConvertToDelimitedString()
        {
            var data = encoding.GetBytes(CsvFiles.GeneralCsv);
            using var stream = new MemoryStream(data);
            using var streamReader = new StreamReader(stream);
            using var csvFile = new CsvFile(streamReader, new CsvConfiguration() { HasHeader = true, CultureInfo = CultureInfo.InvariantCulture });
            
            csvFile.ReadAsync().Wait();
            string result = csvFile.Current.ToString(',');
            
            Assert.IsTrue(result.Contains(","));
            Assert.IsTrue(result.Contains("TG"));
            Assert.IsTrue(result.Contains("Neil"));
        }

        [TestMethod]
        public void CsvRow_ToString_WithCustomDelimiter_ShouldUseCustomDelimiter()
        {
            var data = encoding.GetBytes(CsvFiles.GeneralCsv);
            using var stream = new MemoryStream(data);
            using var streamReader = new StreamReader(stream);
            using var csvFile = new CsvFile(streamReader, new CsvConfiguration() { HasHeader = true, CultureInfo = CultureInfo.InvariantCulture });
            
            csvFile.ReadAsync().Wait();
            string result = csvFile.Current.ToString('|');
            
            Assert.IsTrue(result.Contains("|"));
            Assert.IsFalse(result.Contains(","));
        }

        [TestMethod]
        public void CsvRow_EncryptDecrypt_BasicFunctionality()
        {
            string key = "Test encryption key for CSV";
            
            CsvCryptoHandler handler = new();
            string originalValue = "TestData";
            string encrypted = handler.EncryptValue(originalValue, key);
            string decrypted = handler.DecryptValue(encrypted, key);
            
            Assert.IsTrue(originalValue != encrypted);
            Assert.IsTrue(originalValue == decrypted);
        }

        [TestMethod]
        public void CsvRow_EncryptionExclusions_BasicTest()
        {
            // Test that encryption handler works
            string key = "Test encryption key for CSV";
            
            CsvCryptoHandler handler = new();
            string value1 = "Value1";
            string value2 = "Value2";
            
            string encrypted1 = handler.EncryptValue(value1, key);
            string encrypted2 = handler.EncryptValue(value2, key);
            
            Assert.IsTrue(value1 != encrypted1);
            Assert.IsTrue(value2 != encrypted2);
        }
    }
}
