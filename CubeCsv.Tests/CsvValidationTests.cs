using CubeCsv.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Globalization;
using System.IO;
using System.Text;

namespace CubeCsv.Tests
{
    [TestClass]
    public class CsvValidationTests
    {
        private ASCIIEncoding encoding = new ASCIIEncoding();

        [TestMethod]
        public void CsvValidTest()
        {            
            var data = encoding.GetBytes(CsvFiles.GeneralCsv);
            using (var stream = new MemoryStream(data))
            {
                using (var streamReader = new StreamReader(stream))
                using (var csv = new CsvFile(streamReader, CultureInfo.InvariantCulture))
                {
                    Assert.IsTrue(csv.Validate(
                        new CsvFieldSchema(HeaderNames.Department, typeof(string), 3),
                        new CsvFieldSchema(HeaderNames.FirstName, typeof(string)),
                        new CsvFieldSchema(HeaderNames.LastName, typeof(string), 12),
                        new CsvFieldSchema(HeaderNames.DateOrBirth, typeof(DateTime)),
                        new CsvFieldSchema(HeaderNames.Age, typeof(int)),
                        new CsvFieldSchema(HeaderNames.Worth, typeof(float))).Success);
                }
            }
        }
        [TestMethod]
        public void CsvInvalidIntegerColumnTest()
        {
            var data = encoding.GetBytes(CsvFiles.InvalidFieldCsv);
            using (var stream = new MemoryStream(data))
            {
                using (var streamReader = new StreamReader(stream))
                using (var csv = new CsvFile(streamReader, CultureInfo.InvariantCulture))
                {
                    var result = csv.Validate(
                        new CsvFieldSchema(HeaderNames.Department, typeof(string), 3),
                        new CsvFieldSchema(HeaderNames.FirstName, typeof(string)),
                        new CsvFieldSchema(HeaderNames.LastName, typeof(string), 12),
                        new CsvFieldSchema(HeaderNames.Age, typeof(int)));
                    Assert.IsTrue(result.HasErrors && result.Errors.Count > 0);
                    Assert.IsTrue(result.Errors[0] == "Column at the index 3 is a type mismatch");
                }
            }
        }
    }
}
