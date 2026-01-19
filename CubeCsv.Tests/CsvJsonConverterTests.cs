using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Globalization;
using System.IO;
using System.Text;

namespace CubeCsv.Tests
{
    [TestClass]
    public class CsvJsonConverterTests
    {
        private readonly ASCIIEncoding encoding = new();

        [TestMethod]
        public void CsvJsonConverter_BasicFunctionality_ShouldWork()
        {
            // ConvertToJson uses internal processing, test basic row to JSON conversion
            var data = encoding.GetBytes(CsvFiles.GeneralCsv);
            using var stream = new MemoryStream(data);
            using var streamReader = new StreamReader(stream);
            using var csvFile = new CsvFile(streamReader, new CsvConfiguration() { HasHeader = true, CultureInfo = CultureInfo.InvariantCulture });
            
            csvFile.ReadAsync().Wait();
            string json = csvFile.Current.ToJson(csvFile.Header);
            
            Assert.IsNotNull(json);
            Assert.IsTrue(json.StartsWith("{"));
            Assert.IsTrue(json.EndsWith("}"));
            Assert.IsTrue(json.Contains("Department"));
        }
    }
}
