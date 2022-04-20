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
            ASCIIEncoding encoding = new ASCIIEncoding();
            var data = encoding.GetBytes(CsvFiles.ParserCsv);
            using (var stream = new MemoryStream(data))
            {
                using (var streamReader = new StreamReader(stream))
                using (var csvReader = new CsvReader(streamReader, CultureInfo.InvariantCulture))
                {
                    CsvFieldHeader departmentHeader = csvReader.Header[HeaderNames.Department];
                    Assert.IsTrue(departmentHeader.Schema.Name == HeaderNames.Department);
                    Assert.IsTrue(departmentHeader.Ordinal == 0);
                    Assert.IsTrue(departmentHeader.Schema.Type == typeof(string));

                    CsvFieldHeader firstNameHeader = csvReader.Header[HeaderNames.FirstName];
                    Assert.IsTrue(firstNameHeader.Schema.Name == HeaderNames.FirstName);
                    Assert.IsTrue(firstNameHeader.Ordinal == 1);
                    Assert.IsTrue(firstNameHeader.Schema.Type == typeof(string));

                    CsvFieldHeader lastNameHeader = csvReader.Header[HeaderNames.LastName];
                    Assert.IsTrue(lastNameHeader.Schema.Name == HeaderNames.LastName);
                    Assert.IsTrue(lastNameHeader.Ordinal == 2);
                    Assert.IsTrue(lastNameHeader.Schema.Length == 12);
                    Assert.IsTrue(lastNameHeader.Schema.Type == typeof(string));

                    CsvFieldHeader dateOrBirthHeader = csvReader.Header[HeaderNames.DateOrBirth];
                    Assert.IsTrue(dateOrBirthHeader.Schema.Name == HeaderNames.DateOrBirth);
                    Assert.IsTrue(dateOrBirthHeader.Ordinal == 3);
                    Assert.IsTrue(dateOrBirthHeader.Schema.Type == typeof(DateTime));

                    CsvFieldHeader ageHeader = csvReader.Header[HeaderNames.Age];
                    Assert.IsTrue(ageHeader.Schema.Name == HeaderNames.Age);
                    Assert.IsTrue(ageHeader.Ordinal == 4);
                    Assert.IsTrue(ageHeader.Schema.Type == typeof(int));

                    CsvFieldHeader worthHeader = csvReader.Header[HeaderNames.Worth];
                    Assert.IsTrue(worthHeader.Schema.Name == HeaderNames.Worth);
                    Assert.IsTrue(worthHeader.Ordinal == 5);
                    Assert.IsTrue(worthHeader.Schema.Type == typeof(float));
                }
            }
        }
    }
}
