using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Globalization;
using System.IO;
using System.Text;

namespace CubeCsv.Tests
{
    [TestClass]
    public class CsvHashGeneratorTests
    {
        private readonly ASCIIEncoding encoding = new();

        [TestMethod]
        public void CsvHashGenerator_AddHashToRow_ShouldGenerateSHA256Hash()
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
    }
}
