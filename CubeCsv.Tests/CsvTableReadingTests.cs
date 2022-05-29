using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CubeCsv.Tests
{
    [TestClass]
    public class CsvTableReadingTests
    {
        [TestMethod]
        public void CsvTableReadingTest()
        {
            CsvDbContext context = new CsvDbContext();
            context.Seed();

            TableDirect tableDirect = new TableDirect(
                CsvSqlConstants.EmployeesTableName, 
                context.CreateConnection(),
                string.Empty,
                new CsvConfiguration() { ColumnExlusions = new() { "Id" } });

            tableDirect = tableDirect.ReadRowsFromTableAsync().Result;

            Assert.IsTrue(tableDirect.Header[0].Ordinal == 0);
            Assert.IsTrue(tableDirect.Header[0].Schema.Name == CsvSqlConstants.FirstName);
            Assert.IsTrue(tableDirect.Header[0].Schema.Type == typeof(string));            

            Assert.IsTrue(tableDirect.Header[1].Ordinal == 1);
            Assert.IsTrue(tableDirect.Header[1].Schema.Name == CsvSqlConstants.LastName);
            Assert.IsTrue(tableDirect.Header[1].Schema.Type == typeof(string));            

            Assert.IsTrue(tableDirect.Header[2].Ordinal == 2);
            Assert.IsTrue(tableDirect.Header[2].Schema.Name == CsvSqlConstants.Age);
            Assert.IsTrue(tableDirect.Header[2].Schema.Type == typeof(long));

            Assert.IsTrue(tableDirect.Header[3].Ordinal == 3);
            Assert.IsTrue(tableDirect.Header[3].Schema.Name == CsvSqlConstants.DateOfBirth);
            Assert.IsTrue(tableDirect.Header[3].Schema.Type == typeof(string));
        }
    }
}
