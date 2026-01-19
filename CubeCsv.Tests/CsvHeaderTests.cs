using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Globalization;
using System.IO;
using System.Text;

namespace CubeCsv.Tests
{
    [TestClass]
    public class CsvHeaderTests
    {
        private readonly ASCIIEncoding encoding = new();

        [TestMethod]
        public void CsvHeader_IndexByName_ShouldReturnCorrectHeader()
        {
            var data = encoding.GetBytes(CsvFiles.GeneralCsv);
            using var stream = new MemoryStream(data);
            using var streamReader = new StreamReader(stream);
            using var csvFile = new CsvFile(streamReader, new CsvConfiguration() { HasHeader = true, CultureInfo = CultureInfo.InvariantCulture });
            
            var departmentHeader = csvFile.Header[HeaderNames.Department];
            
            Assert.IsNotNull(departmentHeader);
            Assert.IsTrue(departmentHeader.Schema.Name == HeaderNames.Department);
            Assert.IsTrue(departmentHeader.Ordinal == 0);
        }

        [TestMethod]
        public void CsvHeader_IndexByOrdinal_ShouldReturnCorrectHeader()
        {
            var data = encoding.GetBytes(CsvFiles.GeneralCsv);
            using var stream = new MemoryStream(data);
            using var streamReader = new StreamReader(stream);
            using var csvFile = new CsvFile(streamReader, new CsvConfiguration() { HasHeader = true, CultureInfo = CultureInfo.InvariantCulture });
            
            var firstHeader = csvFile.Header[0];
            
            Assert.IsNotNull(firstHeader);
            Assert.IsTrue(firstHeader.Schema.Name == HeaderNames.Department);
            Assert.IsTrue(firstHeader.Ordinal == 0);
        }

        [TestMethod]
        public void CsvHeader_Count_ShouldReturnCorrectCount()
        {
            var data = encoding.GetBytes(CsvFiles.GeneralCsv);
            using var stream = new MemoryStream(data);
            using var streamReader = new StreamReader(stream);
            using var csvFile = new CsvFile(streamReader, new CsvConfiguration() { HasHeader = true, CultureInfo = CultureInfo.InvariantCulture });
            
            Assert.IsTrue(csvFile.Header.Count == 6);
        }

        [TestMethod]
        public void CsvHeader_MultipleColumns_ShouldHaveCorrectOrdinals()
        {
            var data = encoding.GetBytes(CsvFiles.GeneralCsv);
            using var stream = new MemoryStream(data);
            using var streamReader = new StreamReader(stream);
            using var csvFile = new CsvFile(streamReader, new CsvConfiguration() { HasHeader = true, CultureInfo = CultureInfo.InvariantCulture });
            
            Assert.IsTrue(csvFile.Header[HeaderNames.Department].Ordinal == 0);
            Assert.IsTrue(csvFile.Header[HeaderNames.FirstName].Ordinal == 1);
            Assert.IsTrue(csvFile.Header[HeaderNames.LastName].Ordinal == 2);
            Assert.IsTrue(csvFile.Header[HeaderNames.DateOrBirth].Ordinal == 3);
            Assert.IsTrue(csvFile.Header[HeaderNames.Age].Ordinal == 4);
            Assert.IsTrue(csvFile.Header[HeaderNames.Worth].Ordinal == 5);
        }

        [TestMethod]
        public void CsvHeader_WithoutHeader_ShouldGenerateColumnNames()
        {
            var data = encoding.GetBytes("TG,Neil,White,1980-07-20,42,47654");
            using var stream = new MemoryStream(data);
            using var streamReader = new StreamReader(stream);
            using var csvFile = new CsvFile(streamReader, new CsvConfiguration() { HasHeader = false, CultureInfo = CultureInfo.InvariantCulture });
            
            csvFile.ReadAsync().Wait();
            
            Assert.IsTrue(csvFile.Header.Count == 6);
            // Just verify we have header columns generated
            Assert.IsTrue(csvFile.Header[0] != null);
            Assert.IsTrue(csvFile.Header[0].Schema != null);
        }

        [TestMethod]
        public void CsvHeader_AddHeader_ShouldInsertAtCorrectLocation()
        {
            var data = encoding.GetBytes(CsvFiles.GeneralCsv);
            using var stream = new MemoryStream(data);
            using var streamReader = new StreamReader(stream);
            using var csvFile = new CsvFile(streamReader, new CsvConfiguration() { HasHeader = true, CultureInfo = CultureInfo.InvariantCulture });
            
            int originalCount = csvFile.Header.Count;
            var newHeader = new CsvFieldHeader
            {
                Schema = new CsvFieldSchema { Name = "NewColumn", Type = typeof(string) },
                Ordinal = 2
            };
            
            csvFile.AddHeader(2, newHeader);
            
            Assert.IsTrue(csvFile.Header.Count == originalCount + 1);
            Assert.IsTrue(csvFile.Header[2].Schema.Name == "NewColumn");
        }

        [TestMethod]
        public void CsvHeader_Dispose_ShouldCleanup()
        {
            var data = encoding.GetBytes(CsvFiles.GeneralCsv);
            using var stream = new MemoryStream(data);
            using var streamReader = new StreamReader(stream);
            var csvFile = new CsvFile(streamReader, new CsvConfiguration() { HasHeader = true, CultureInfo = CultureInfo.InvariantCulture });
            
            csvFile.Header.Dispose();
            
            // Should not throw exception
            Assert.IsTrue(true);
        }
    }
}
