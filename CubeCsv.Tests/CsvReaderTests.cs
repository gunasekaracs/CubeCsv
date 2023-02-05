using CubeCsv.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Globalization;
using System.IO;
using System.Text;

namespace CubeCsv.Tests
{
    [TestClass]
    public class CsvReaderTests
    {
        private ASCIIEncoding encoding = new ASCIIEncoding();

        [TestMethod]
        public void CsvReaderTest()
        {
            var data = encoding.GetBytes(CsvFiles.GeneralCsv);
            using var stream = new MemoryStream(data);
            using var streamReader = new StreamReader(stream);
            using var csvFile = new CsvFile(streamReader, new CsvConfiguration() { HasHeader = true, CultureInfo = CultureInfo.InvariantCulture });
            for (int i = 0; i < 3; i++)
                csvFile.ReadAsync().Wait();
            Assert.IsTrue(csvFile.GetValueAsString(HeaderNames.Department) == "BBS");
            Assert.IsTrue(csvFile.GetValue<string>(HeaderNames.Department) == "BBS");
            Assert.IsTrue(csvFile.GetValueAsString(HeaderNames.FirstName) == "Dilshan");
            Assert.IsTrue(csvFile.GetValue<string>(HeaderNames.FirstName) == "Dilshan");
            Assert.IsTrue(csvFile.GetValueAsString(HeaderNames.LastName) == "Amarasinghe");
            Assert.IsTrue(csvFile.GetValue<string>(HeaderNames.LastName) == "Amarasinghe");
            Assert.IsTrue(DateTime.Parse(csvFile.GetValueAsString(HeaderNames.DateOrBirth) ?? "1980-07-20") == DateTime.Parse("07-05-2002"));
            Assert.IsTrue(csvFile.GetValue<DateTime>(HeaderNames.DateOrBirth) == DateTime.Parse("07-05-2002"));
            Assert.IsTrue(int.Parse(csvFile.GetValueAsString(HeaderNames.Age) ?? "0") == 34);
            Assert.IsTrue(csvFile.GetValue<int>(HeaderNames.Age) == 34);
            Assert.IsTrue(float.Parse(csvFile.GetValueAsString(HeaderNames.Worth) ?? "0.0") == 214748364700000);
            Assert.IsTrue(csvFile.GetValue<float>(HeaderNames.Worth) == 214748364700000f);
            Assert.ThrowsException<CsvInvalidCastException>(() => csvFile.GetValue<DateTime>(HeaderNames.FirstName) == DateTime.Parse("07-05-2002"));
        }
        [TestMethod]
        public void CsvReadEmptyCsvFileTest()
        {
            var data = encoding.GetBytes(CsvFiles.EmptyRowsCsv);
            using var stream = new MemoryStream(data);
            using var streamReader = new StreamReader(stream);
            using var tableDirect = new CsvFile(streamReader, new CsvConfiguration() { HasHeader = true, CultureInfo = CultureInfo.InvariantCulture });
            tableDirect.ReadAsync().Wait();
            Assert.IsTrue(tableDirect.GetValue<string>(HeaderNames.FullName) == "Test");
            Assert.IsTrue(tableDirect.GetValueAsString(HeaderNames.BirthDate) == string.Empty);
            Assert.IsTrue(tableDirect.GetValueAsString(HeaderNames.CustomerCode) == string.Empty);
            tableDirect.ReadAsync().Wait();
            Assert.IsTrue(tableDirect.GetValue<string>(HeaderNames.FullName) == "Dilshan");
            Assert.IsTrue(tableDirect.GetValueAsString(HeaderNames.BirthDate) == string.Empty);
            Assert.IsTrue(tableDirect.GetValueAsString(HeaderNames.CustomerCode) == string.Empty);
            tableDirect.ReadAsync().Wait();
            Assert.IsTrue(tableDirect.GetValue<string>(HeaderNames.CustomerCode) == "CON2429");
            Assert.IsTrue(tableDirect.GetValueAsString(HeaderNames.BirthDate) == string.Empty);
            Assert.IsTrue(tableDirect.GetValueAsString(HeaderNames.FullName) == string.Empty);
            tableDirect.ReadAsync().Wait();
            Assert.IsTrue(tableDirect.GetValue<DateTime>(HeaderNames.BirthDate) == DateTime.Parse("2014-03-13"));
            Assert.IsTrue(tableDirect.GetValueAsString(HeaderNames.CustomerCode) == string.Empty);
            Assert.IsTrue(tableDirect.GetValueAsString(HeaderNames.FullName) == string.Empty);
        }
    }
}