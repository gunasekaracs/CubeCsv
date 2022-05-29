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
                using (var csvReader = new CsvStreamReader(streamReader, CultureInfo.InvariantCulture))
                {
                    for (int i = 0; i < 3; i++)
                        csvReader.ReadAsync().Wait();
                    Assert.IsTrue(csvReader.GetValueAsString(HeaderNames.Department) == "BBS");
                    Assert.IsTrue(csvReader.GetValue<string>(HeaderNames.Department) == "BBS");
                    Assert.IsTrue(csvReader.GetValueAsString(HeaderNames.FirstName) == "Dilshan");
                    Assert.IsTrue(csvReader.GetValue<string>(HeaderNames.FirstName) == "Dilshan");
                    Assert.IsTrue(csvReader.GetValueAsString(HeaderNames.LastName) == "Amarasinghe");
                    Assert.IsTrue(csvReader.GetValue<string>(HeaderNames.LastName) == "Amarasinghe");
                    Assert.IsTrue(DateTime.Parse(csvReader.GetValueAsString(HeaderNames.DateOrBirth) ?? "1980-07-20") == DateTime.Parse("07-05-2002"));
                    Assert.IsTrue(csvReader.GetValue<DateTime>(HeaderNames.DateOrBirth) == DateTime.Parse("07-05-2002"));
                    Assert.IsTrue(int.Parse(csvReader.GetValueAsString(HeaderNames.Age) ?? "0") == 34);
                    Assert.IsTrue(csvReader.GetValue<int>(HeaderNames.Age) == 34);
                    Assert.IsTrue(float.Parse(csvReader.GetValueAsString(HeaderNames.Worth) ?? "0.0") == 214748364700000);
                    Assert.IsTrue(csvReader.GetValue<float>(HeaderNames.Worth) == 214748364700000f);
                    Assert.ThrowsException<CsvInvalidCastException>(() => csvReader.GetValue<DateTime>(HeaderNames.FirstName) == DateTime.Parse("07-05-2002"));
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
                using (var csvReader = new CsvStreamReader(streamReader, CultureInfo.InvariantCulture))
                {
                    csvReader.ReadAsync().Wait();
                    Assert.IsTrue(csvReader.GetValue<string>(HeaderNames.FullName) == "Test");
                    Assert.IsTrue(csvReader.GetValueAsString(HeaderNames.BirthDate) == string.Empty);
                    Assert.IsTrue(csvReader.GetValueAsString(HeaderNames.CustomerCode) == string.Empty);
                    csvReader.ReadAsync().Wait();
                    Assert.IsTrue(csvReader.GetValue<string>(HeaderNames.FullName) == "Dilshan");
                    Assert.IsTrue(csvReader.GetValueAsString(HeaderNames.BirthDate) == string.Empty);
                    Assert.IsTrue(csvReader.GetValueAsString(HeaderNames.CustomerCode) == string.Empty);
                    csvReader.ReadAsync().Wait();
                    Assert.IsTrue(csvReader.GetValue<string>(HeaderNames.CustomerCode) == "CON2429");
                    Assert.IsTrue(csvReader.GetValueAsString(HeaderNames.BirthDate) == string.Empty);
                    Assert.IsTrue(csvReader.GetValueAsString(HeaderNames.FullName) == string.Empty);
                    csvReader.ReadAsync().Wait();
                    Assert.IsTrue(csvReader.GetValue<DateTime>(HeaderNames.BirthDate) == DateTime.Parse("2014-03-13"));
                    Assert.IsTrue(csvReader.GetValueAsString(HeaderNames.CustomerCode) == string.Empty);
                    Assert.IsTrue(csvReader.GetValueAsString(HeaderNames.FullName) == string.Empty);
                }
            }
        }
    }
}