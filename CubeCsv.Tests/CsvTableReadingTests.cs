using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

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

            int count = tableDirect.CountAsync().Result;
            Assert.IsTrue(count == 5);

            while (tableDirect.ReadAsync().Result)
            {
                if (tableDirect.Location == 0)
                {
                    Assert.IsTrue(tableDirect.GetValue<string>(CsvSqlConstants.FirstName) == "Charith");
                    Assert.IsTrue(tableDirect.GetValue<string>(CsvSqlConstants.LastName) == "Gunasekara");
                    Assert.IsTrue(tableDirect.GetValue<long>(CsvSqlConstants.Age) == 42);
                    Assert.IsTrue(tableDirect.GetValueAsString(CsvSqlConstants.DateOfBirth) == "1980-07-20");
                }
                if (tableDirect.Location == 1)
                {
                    Assert.IsTrue(tableDirect.GetValue<string>(CsvSqlConstants.FirstName) == "Dilshan");
                    Assert.IsTrue(tableDirect.GetValue<string>(CsvSqlConstants.LastName) == "Amarasinghe");
                    Assert.IsTrue(tableDirect.GetValue<long>(CsvSqlConstants.Age) == 34);
                    Assert.IsTrue(tableDirect.GetValueAsString(CsvSqlConstants.DateOfBirth) == "1988-01-10");
                }
                if (tableDirect.Location == 2)
                {
                    Assert.IsTrue(tableDirect.GetValueAsString(CsvSqlConstants.FirstName) == "Kumanan");
                    Assert.IsTrue(tableDirect.GetValueAsString(CsvSqlConstants.LastName) == "Panchalingam");
                    Assert.IsTrue(tableDirect.GetValue<long>(CsvSqlConstants.Age) == 30);
                    Assert.IsTrue(tableDirect.GetValueAsString(CsvSqlConstants.DateOfBirth) == "1992-05-02");
                }
                if (tableDirect.Location == 3)
                {
                    Assert.IsTrue(tableDirect.GetValueAsString(CsvSqlConstants.FirstName) == "Azmina");
                    Assert.IsTrue(tableDirect.GetValueAsString(CsvSqlConstants.LastName) == "Mohomadeen");
                    Assert.IsTrue(tableDirect.GetValue<long>(CsvSqlConstants.Age) == 30);
                    Assert.IsTrue(tableDirect.GetValueAsString(CsvSqlConstants.DateOfBirth) == "1992-05-20");
                }
                if (tableDirect.Location == 4)
                {
                    Assert.IsTrue(tableDirect.GetValueAsString(CsvSqlConstants.FirstName) == "Eswar");
                    Assert.IsTrue(tableDirect.GetValueAsString(CsvSqlConstants.LastName) == "Raj");
                    Assert.IsTrue(tableDirect.GetValue<long>(CsvSqlConstants.Age) == 30);
                    Assert.IsTrue(tableDirect.GetValueAsString(CsvSqlConstants.DateOfBirth) == "1992-08-05");
                }
            }
        }
    }
}
