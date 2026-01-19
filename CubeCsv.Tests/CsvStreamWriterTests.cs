using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Globalization;
using System.IO;
using System.Text;

namespace CubeCsv.Tests
{
    [TestClass]
    public class CsvStreamWriterTests
    {
        private readonly ASCIIEncoding encoding = new();

        [TestMethod]
        public void CsvStreamWriter_BasicFunctionality_ShouldWork()
        {
            // CsvStreamWriter is used internally, test basic row writing
            var data = encoding.GetBytes(CsvFiles.GeneralCsv);
            using var inputStream = new MemoryStream(data);
            using var inputReader = new StreamReader(inputStream);
            using var csvFile = new CsvFile(inputReader, new CsvConfiguration() { HasHeader = true, CultureInfo = CultureInfo.InvariantCulture });
            
            csvFile.ReadAsync().Wait();
            string rowString = csvFile.Current.ToString(',');
            
            Assert.IsNotNull(rowString);
            Assert.IsTrue(rowString.Contains(","));
        }
    }
}
