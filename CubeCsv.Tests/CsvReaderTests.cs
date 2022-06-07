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
            using (var stream = new MemoryStream(data))
            {
                using (var streamReader = new StreamReader(stream))
                using (var tableDirect = new CsvFile(streamReader, new CsvConfiguration() { HasHeader = true, CultureInfo = CultureInfo.InvariantCulture }))
                {
                    for (int i = 0; i < 3; i++)
                        tableDirect.ReadAsync().Wait();
                    Assert.IsTrue(tableDirect.GetValueAsString(HeaderNames.Department) == "BBS");
                    Assert.IsTrue(tableDirect.GetValue<string>(HeaderNames.Department) == "BBS");
                    Assert.IsTrue(tableDirect.GetValueAsString(HeaderNames.FirstName) == "Dilshan");
                    Assert.IsTrue(tableDirect.GetValue<string>(HeaderNames.FirstName) == "Dilshan");
                    Assert.IsTrue(tableDirect.GetValueAsString(HeaderNames.LastName) == "Amarasinghe");
                    Assert.IsTrue(tableDirect.GetValue<string>(HeaderNames.LastName) == "Amarasinghe");
                    Assert.IsTrue(DateTime.Parse(tableDirect.GetValueAsString(HeaderNames.DateOrBirth) ?? "1980-07-20") == DateTime.Parse("07-05-2002"));
                    Assert.IsTrue(tableDirect.GetValue<DateTime>(HeaderNames.DateOrBirth) == DateTime.Parse("07-05-2002"));
                    Assert.IsTrue(int.Parse(tableDirect.GetValueAsString(HeaderNames.Age) ?? "0") == 34);
                    Assert.IsTrue(tableDirect.GetValue<int>(HeaderNames.Age) == 34);
                    Assert.IsTrue(float.Parse(tableDirect.GetValueAsString(HeaderNames.Worth) ?? "0.0") == 214748364700000);
                    Assert.IsTrue(tableDirect.GetValue<float>(HeaderNames.Worth) == 214748364700000f);
                    Assert.ThrowsException<CsvInvalidCastException>(() => tableDirect.GetValue<DateTime>(HeaderNames.FirstName) == DateTime.Parse("07-05-2002"));
                }
            }
        }
        [TestMethod]
        public void CsvReadEmptyCsvFileTest()
        {
            var data = encoding.GetBytes(CsvFiles.EmptyRowsCsv);
            using (var stream = new MemoryStream(data))
            {
                using (var streamReader = new StreamReader(stream))
                using (var tableDirect = new CsvFile(streamReader, new CsvConfiguration() { HasHeader = true, CultureInfo = CultureInfo.InvariantCulture }))
                {
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
    }
}