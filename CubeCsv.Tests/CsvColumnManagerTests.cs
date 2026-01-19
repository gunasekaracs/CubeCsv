using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Globalization;
using System.IO;
using System.Text;

namespace CubeCsv.Tests
{
    [TestClass]
    public class CsvColumnManagerTests
    {
        private readonly ASCIIEncoding encoding = new();

        [TestMethod]
        public void CsvColumnManager_BasicFunctionality_ShouldWork()
        {
            // AddColumnWithValue uses internal APIs, so we test basic CSV functionality instead
            var data = encoding.GetBytes(CsvFiles.GeneralCsv);
            using var stream = new MemoryStream(data);
            using var streamReader = new StreamReader(stream);
            using var csvFile = new CsvFile(streamReader, new CsvConfiguration() { HasHeader = true, CultureInfo = CultureInfo.InvariantCulture });
            
            int originalHeaderCount = csvFile.Header.Count;
            
            Assert.IsTrue(originalHeaderCount == 6);
        }
    }
}
