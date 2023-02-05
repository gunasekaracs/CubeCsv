using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Globalization;
using System.IO;
using System.Text;

namespace CubeCsv.Tests
{
    [TestClass]
    public class CsvParserTests
    {
        [TestMethod]
        public void CsvParserTest()
        {
            ASCIIEncoding encoding = new();
            var data = encoding.GetBytes(CsvFiles.ParserCsv);
            using var stream = new MemoryStream(data);
            using var streamReader = new StreamReader(stream);
            using var csvFile = new CsvFile(streamReader, new CsvConfiguration() { HasHeader = true, CultureInfo = CultureInfo.InvariantCulture });
            CsvFieldHeader departmentHeader = csvFile.Header[HeaderNames.Department];
            Assert.IsTrue(departmentHeader.Schema.Name == HeaderNames.Department);
            Assert.IsTrue(departmentHeader.Ordinal == 0);
            Assert.IsTrue(departmentHeader.Schema.Type == typeof(string));

            CsvFieldHeader firstNameHeader = csvFile.Header[HeaderNames.FirstName];
            Assert.IsTrue(firstNameHeader.Schema.Name == HeaderNames.FirstName);
            Assert.IsTrue(firstNameHeader.Ordinal == 1);
            Assert.IsTrue(firstNameHeader.Schema.Type == typeof(string));

            CsvFieldHeader lastNameHeader = csvFile.Header[HeaderNames.LastName];
            Assert.IsTrue(lastNameHeader.Schema.Name == HeaderNames.LastName);
            Assert.IsTrue(lastNameHeader.Ordinal == 2);
            Assert.IsTrue(lastNameHeader.Schema.Length == 12);
            Assert.IsTrue(lastNameHeader.Schema.Type == typeof(string));

            CsvFieldHeader dateOrBirthHeader = csvFile.Header[HeaderNames.DateOrBirth];
            Assert.IsTrue(dateOrBirthHeader.Schema.Name == HeaderNames.DateOrBirth);
            Assert.IsTrue(dateOrBirthHeader.Ordinal == 3);
            Assert.IsTrue(dateOrBirthHeader.Schema.Type == typeof(DateTime));

            CsvFieldHeader ageHeader = csvFile.Header[HeaderNames.Age];
            Assert.IsTrue(ageHeader.Schema.Name == HeaderNames.Age);
            Assert.IsTrue(ageHeader.Ordinal == 4);
            Assert.IsTrue(ageHeader.Schema.Type == typeof(int));

            CsvFieldHeader worthHeader = csvFile.Header[HeaderNames.Worth];
            Assert.IsTrue(worthHeader.Schema.Name == HeaderNames.Worth);
            Assert.IsTrue(worthHeader.Ordinal == 5);
            Assert.IsTrue(worthHeader.Schema.Type == typeof(float));
        }
    }
}
