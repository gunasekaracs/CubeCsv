using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
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
            using var stream = new MemoryStream(data);
            using var streamReader = new StreamReader(stream);
            using var csv = new CsvFile(streamReader, new CsvConfiguration() { HasHeader = true, CultureInfo = CultureInfo.InvariantCulture });
            Assert.IsTrue(csv.ValidateSchema(
                new CsvFieldSchema(HeaderNames.Department, typeof(string), 3),
                new CsvFieldSchema(HeaderNames.FirstName, typeof(string)),
                new CsvFieldSchema(HeaderNames.LastName, typeof(string), 12),
                new CsvFieldSchema(HeaderNames.DateOrBirth, typeof(DateTime)),
                new CsvFieldSchema(HeaderNames.Age, typeof(int)),
                new CsvFieldSchema(HeaderNames.Worth, typeof(float))).Success);
        }

        [TestMethod]
        public void CsvInvalidIntegerColumnTest()
        {
            var data = encoding.GetBytes(CsvFiles.InvalidFieldCsv);
            using var stream = new MemoryStream(data);
            using var streamReader = new StreamReader(stream);
            using var csv = new CsvFile(streamReader, new CsvConfiguration() { HasHeader = true, CultureInfo = CultureInfo.InvariantCulture });
            var result = csv.ValidateSchema(
                new CsvFieldSchema(HeaderNames.Department, typeof(string), 3),
                new CsvFieldSchema(HeaderNames.FirstName, typeof(string)),
                new CsvFieldSchema(HeaderNames.LastName, typeof(string), 12),
                new CsvFieldSchema(HeaderNames.Age, typeof(int)));
            Assert.IsTrue(result.HasErrors && result.Errors.Count > 0);
            Assert.IsTrue(result.Errors[0].Error == "Column at the index 3 is a type mismatch");
        }

        [TestMethod]
        public void IncorrectDataTypeOnLastColumnsLastRowTest()
        {
            var data = encoding.GetBytes(CsvFiles.IncorrectDataTypeOnLastColumnsLastRow);
            using var stream = new MemoryStream(data);
            using var streamReader = new StreamReader(stream);
            using var csv = new CsvFile(streamReader, new CsvConfiguration()
            {
                CultureInfo = CultureInfo.InvariantCulture,
                BreakOnError = false,
                Schema = new CsvSchema()
                    {
                        new CsvFieldSchema()
                        {
                             Type=typeof(string),
                             Validator = new CsvFieldValidator()
                             {
                                 Type = CsvFieldValidator.ValidatorType.Regex,
                                 Description="Format of the department code should be three letters and three dights seperated by dash, eg ABC-123",
                                 RegularExpression="^[a-zA-Z]{3}-[0-9]{3}"
                             }
                        }
                    }
            });
            Assert.IsTrue(csv.CountAsync().Result == 10);
            Assert.IsTrue(csv.Errors.Count == 1);
            var error = csv.Errors.SingleOrDefault();
            Assert.IsTrue(string.Compare(error?.Error, "Error at the row 10, column 0, value [543] is not in the correct format Format of the department code should be three letters and three dights seperated by dash, eg ABC-123") == 0);
        }

        [TestMethod]
        public void IncorrectDataTypeOnLastColumnsLastRowExcludeDataTest()
        {
            var data = encoding.GetBytes(CsvFiles.IncorrectDataTypeOnLastColumnsLastRow);
            using var stream = new MemoryStream(data);
            using var streamReader = new StreamReader(stream);
            using var csv = new CsvFile(streamReader, new CsvConfiguration()
            {
                CultureInfo = CultureInfo.InvariantCulture,
                BreakOnError = false,
                IncludeDataInLogs = false,
                Schema = new CsvSchema()
                    {
                        new CsvFieldSchema()
                        {
                             Type=typeof(string),
                             Validator = new CsvFieldValidator()
                             {
                                 Type = CsvFieldValidator.ValidatorType.Regex,
                                 Description="Format of the department code should be three letters and three dights seperated by dash, eg ABC-123",
                                 RegularExpression="^[a-zA-Z]{3}-[0-9]{3}"
                             }
                        }
                    }
            });
            Assert.IsTrue(csv.CountAsync().Result == 10);
            Assert.IsTrue(csv.Errors.Count == 1);
            var error = csv.Errors.SingleOrDefault();
            Assert.IsTrue(string.Compare(error?.Error, "Error at the row 10, column 0, value is not in the correct format Format of the department code should be three letters and three dights seperated by dash, eg ABC-123") == 0);
        }

        [TestMethod]
        public void WithQualifyingQuotesAndCommaInTheCellValuesTest()
        {
            var data = encoding.GetBytes(CsvFiles.WithQualifyingQuotesAndCommaInTheCellValues);
            using var stream = new MemoryStream(data);
            using var streamReader = new StreamReader(stream);
            using var csv = new CsvFile(streamReader, new CsvConfiguration()
            {
                CultureInfo = CultureInfo.InvariantCulture,
                BreakOnError = false,
                Schema = new CsvSchema()
                    {
                        new CsvFieldSchema() { Length= 7, Type = typeof(string) },
                        new CsvFieldSchema() { Length= 7, Type = typeof(string) },
                        new CsvFieldSchema() { Length= 7, Type = typeof(string) },
                        new CsvFieldSchema() { Length= 7, Type = typeof(string) },
                        new CsvFieldSchema() { Length= 10, Type = typeof(string) },
                        new CsvFieldSchema() { Length= 7, Type = typeof(string) },
                        new CsvFieldSchema() { Length= 7, Type = typeof(string) },
                        new CsvFieldSchema() { Length= 7, Type = typeof(string) },
                        new CsvFieldSchema() { Length= 7, Type = typeof(string) },
                        new CsvFieldSchema() { Length= 7, Type = typeof(string) },
                        new CsvFieldSchema() { Length= 7, Type = typeof(string) },
                        new CsvFieldSchema() { Length= 7, Type = typeof(string) },
                        new CsvFieldSchema() { Type = typeof(int) },
                    }
            });
            Assert.IsTrue(csv.CountAsync().Result == 9);
            while (csv.ReadAsync().Result)
            {
                if (csv.Location == 3)
                    Assert.IsTrue(string.Compare("ABC\"DE\"FG", csv.GetValue<string>(4)) == 0);
                if (csv.Location == 4)
                    Assert.IsTrue(string.Compare("ABC\"DEFG", csv.GetValue<string>(4)) == 0);
                if (csv.Location == 5)
                    Assert.IsTrue(string.Compare("ABCD,EFG", csv.GetValue<string>(4)) == 0);
                if (csv.Location == 6)
                    Assert.IsTrue(string.Compare("ABC\"D,EFG", csv.GetValue<string>(4)) == 0);
            }
        }

        [TestMethod]
        public void SomeDataLargerThanTheSchemaLimitsAllowsTest()
        {
            var data = new UTF8Encoding().GetBytes(CsvFiles.SomeDataLargerThanTheSchemaLimitsAllows);
            using var stream = new MemoryStream(data);
            using var streamReader = new StreamReader(stream);
            using var csv = new CsvFile(streamReader, new CsvConfiguration()
            {
                CultureInfo = CultureInfo.InvariantCulture,
                Delimiter = '¤',
                BreakOnError = false,
                Schema = new CsvSchema()
                    {
                        new CsvFieldSchema() { Length= 7, Type = typeof(string) },
                        new CsvFieldSchema() { Length= 7, Type = typeof(string) },
                        new CsvFieldSchema() { Length= 7, Type = typeof(string) },
                        new CsvFieldSchema() { Length= 7, Type = typeof(string) },
                        new CsvFieldSchema() { Length= 7, Type = typeof(string) },
                        new CsvFieldSchema() { Length= 7, Type = typeof(string) },
                        new CsvFieldSchema() { Length= 7, Type = typeof(string) },
                        new CsvFieldSchema() { Type = typeof(int) },
                    }
            });
            Assert.IsTrue(csv.CountAsync().Result == 7);
            Assert.IsTrue(csv.Errors.Count == 3);
            Assert.IsTrue(string.Compare(csv.Errors[0].Error, "Error at the row 2, Provided value [ABCDEFGH] too large to fit in this column 0. Schema length [7] is but length of the value is [8]") == 0);
            Assert.IsTrue(string.Compare(csv.Errors[1].Error, "Error at the row 3, Provided value [ABCDEFGH] too large to fit in this column 0. Schema length [7] is but length of the value is [8]") == 0);
            Assert.IsTrue(string.Compare(csv.Errors[2].Error, "Error at the row 4, Provided value [ABCDEFGH] too large to fit in this column 0. Schema length [7] is but length of the value is [8]") == 0);
        }

        [TestMethod]
        public void DataTypeIssuesTest()
        {
            var data = new UTF8Encoding().GetBytes(CsvFiles.DataTypeIssues);
            using var stream = new MemoryStream(data);
            using var streamReader = new StreamReader(stream);
            using var csv = new CsvFile(streamReader, new CsvConfiguration()
            {
                CultureInfo = CultureInfo.InvariantCulture,
                Delimiter = '¤',
                BreakOnError = false,
                Schema = new CsvSchema()
                    {
                        new CsvFieldSchema() { Length= 7, Type = typeof(string) },
                        new CsvFieldSchema() { Type = typeof(DateTime) },
                        new CsvFieldSchema() { Length= 7, Type = typeof(string) },
                        new CsvFieldSchema() { Type = typeof(double) },
                        new CsvFieldSchema() { Length= 7, Type = typeof(string) },
                        new CsvFieldSchema() { Type = typeof(DateTime) },
                        new CsvFieldSchema() { Length= 7, Type = typeof(string)},
                        new CsvFieldSchema() { Type = typeof(int) },
                    }
            });
            Assert.IsTrue(csv.CountAsync().Result == 7);
            Assert.IsTrue(csv.Errors.Count == 8);

            Assert.IsTrue(string.Compare(csv.Errors[0].Error, "Error at the row 4, column 1, value [2009-06-31] cannot be converted to Date Time") == 0);
            Assert.IsTrue(string.Compare(csv.Errors[1].Error, "Error at the row 4, column 1, value [ABCDEFG] cannot be converted to Date Time") == 0);
            Assert.IsTrue(string.Compare(csv.Errors[2].Error, "Error at the row 4, column 1, value [12.35] cannot be converted to Date Time") == 0);
            Assert.IsTrue(string.Compare(csv.Errors[3].Error, "Error at the row 4, column 1, value [ABCDEFG] cannot be converted to Date Time") == 0);
            Assert.IsTrue(string.Compare(csv.Errors[4].Error, "Error at the row 6, column 1, value [2009-06-15T24:45:30.0000000Z] cannot be converted to Date Time") == 0);
            Assert.IsTrue(string.Compare(csv.Errors[5].Error, "Error at the row 6, column 1, value [ABCDEFG] cannot be converted to Date Time") == 0);
            Assert.IsTrue(string.Compare(csv.Errors[6].Error, "Error at the row 6, column 1, value [12345] cannot be converted to Date Time") == 0);
            Assert.IsTrue(string.Compare(csv.Errors[7].Error, "Error at the row 9, column 7, value [1234A] cannot be converted to int") == 0);
        }

        [TestMethod]
        public void MultiLineDataTest()
        {
            var data = new UTF8Encoding().GetBytes(CsvFiles.MultiLineData);
            using var stream = new MemoryStream(data);
            using var streamReader = new StreamReader(stream);
            using var csv = new CsvFile(streamReader, new CsvConfiguration()
            {
                CultureInfo = CultureInfo.InvariantCulture,
                RemoveLineBreaks = true,
            });
            Assert.IsTrue(csv.CountAsync().Result == 10);
        }

        [TestMethod]
        public void InvalidIntegerAtTheLastCellTest()
        {
            var data = new UTF8Encoding().GetBytes(CsvFiles.InvalidIntegerAtTheLastCell);
            using var stream = new MemoryStream(data);
            using var streamReader = new StreamReader(stream);
            using var csv = new CsvFile(streamReader, new CsvConfiguration()
            {
                CultureInfo = CultureInfo.InvariantCulture,
                BreakOnError = false,
                IncludeDataInLogs = false,
                Schema = new CsvSchema()
                    {
                        new CsvFieldSchema() { Length= 7, Type = typeof(string) },
                        new CsvFieldSchema() { Length= 7, Type = typeof(string) },
                        new CsvFieldSchema() { Length= 7, Type = typeof(string) },
                        new CsvFieldSchema() { Length= 7, Type = typeof(string) },
                        new CsvFieldSchema() { Length= 7, Type = typeof(string) },
                        new CsvFieldSchema() { Length= 7, Type = typeof(string) },
                        new CsvFieldSchema() { Length= 7, Type = typeof(string) },
                        new CsvFieldSchema() { Length= 7, Type = typeof(string) },
                        new CsvFieldSchema() { Length= 7, Type = typeof(string) },
                        new CsvFieldSchema() { Length= 7, Type = typeof(string) },
                        new CsvFieldSchema() { Length= 7, Type = typeof(string) },
                        new CsvFieldSchema() { Length= 10, Type = typeof(string) },
                        new CsvFieldSchema() { Type = typeof(int) },
                    }
            });
            Assert.IsTrue(csv.CountAsync().Result == 10);
            Assert.IsTrue(csv.Errors.Count == 1);
            Assert.IsTrue(string.Compare(csv.Errors[0].Error, "Error at the row 10, column 12, value cannot be converted to int") == 0);
        }
    }
}
