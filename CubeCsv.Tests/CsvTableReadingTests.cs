﻿using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CubeCsv.Tests
{
    [TestClass]
    public class CsvTableReadingTests
    {
        [TestMethod]
        public void CsvTableReadingTest()
        {
            CsvDbContext context = new();
            context.Seed();

            CsvFile csvFile = new(
                CsvSqlConstants.EmployeesTableName,
                context.CreateConnection(),
                string.Empty,
                string.Empty,
                new CsvConfiguration() { ColumnExlusions = new() { "Id" } });

            csvFile = csvFile.ReadRowsFromTableAsync().Result;

            Assert.IsTrue(csvFile.Header[0].Ordinal == 0);
            Assert.IsTrue(csvFile.Header[0].Schema.Name == CsvSqlConstants.FirstName);
            Assert.IsTrue(csvFile.Header[0].Schema.Type == typeof(string));

            Assert.IsTrue(csvFile.Header[1].Ordinal == 1);
            Assert.IsTrue(csvFile.Header[1].Schema.Name == CsvSqlConstants.LastName);
            Assert.IsTrue(csvFile.Header[1].Schema.Type == typeof(string));

            Assert.IsTrue(csvFile.Header[2].Ordinal == 2);
            Assert.IsTrue(csvFile.Header[2].Schema.Name == CsvSqlConstants.Age);
            Assert.IsTrue(csvFile.Header[2].Schema.Type == typeof(long));

            Assert.IsTrue(csvFile.Header[3].Ordinal == 3);
            Assert.IsTrue(csvFile.Header[3].Schema.Name == CsvSqlConstants.DateOfBirth);
            Assert.IsTrue(csvFile.Header[3].Schema.Type == typeof(string));

            int count = csvFile.CountAsync().Result;
            Assert.IsTrue(count == 5);

            while (csvFile.ReadAsync().Result)
            {
                if (csvFile.Location == 0)
                {
                    Assert.IsTrue(csvFile.GetValue<string>(CsvSqlConstants.FirstName) == "Neil");
                    Assert.IsTrue(csvFile.GetValue<string>(CsvSqlConstants.LastName) == "White");
                    Assert.IsTrue(csvFile.GetValue<long>(CsvSqlConstants.Age) == 42);
                    Assert.IsTrue(csvFile.GetValueAsString(CsvSqlConstants.DateOfBirth) == "1980-07-20");
                }
                if (csvFile.Location == 1)
                {
                    Assert.IsTrue(csvFile.GetValue<string>(CsvSqlConstants.FirstName) == "Peter");
                    Assert.IsTrue(csvFile.GetValue<string>(CsvSqlConstants.LastName) == "Bell");
                    Assert.IsTrue(csvFile.GetValue<long>(CsvSqlConstants.Age) == 34);
                    Assert.IsTrue(csvFile.GetValueAsString(CsvSqlConstants.DateOfBirth) == "1988-01-10");
                }
                if (csvFile.Location == 2)
                {
                    Assert.IsTrue(csvFile.GetValueAsString(CsvSqlConstants.FirstName) == "Richard");
                    Assert.IsTrue(csvFile.GetValueAsString(CsvSqlConstants.LastName) == "Johnes");
                    Assert.IsTrue(csvFile.GetValue<long>(CsvSqlConstants.Age) == 30);
                    Assert.IsTrue(csvFile.GetValueAsString(CsvSqlConstants.DateOfBirth) == "1992-05-02");
                }
                if (csvFile.Location == 3)
                {
                    Assert.IsTrue(csvFile.GetValueAsString(CsvSqlConstants.FirstName) == "Nancy");
                    Assert.IsTrue(csvFile.GetValueAsString(CsvSqlConstants.LastName) == "Bella");
                    Assert.IsTrue(csvFile.GetValue<long>(CsvSqlConstants.Age) == 30);
                    Assert.IsTrue(csvFile.GetValueAsString(CsvSqlConstants.DateOfBirth) == "1992-05-20");
                }
                if (csvFile.Location == 4)
                {
                    Assert.IsTrue(csvFile.GetValueAsString(CsvSqlConstants.FirstName) == "Bob");
                    Assert.IsTrue(csvFile.GetValueAsString(CsvSqlConstants.LastName) == "Way");
                    Assert.IsTrue(csvFile.GetValue<long>(CsvSqlConstants.Age) == 30);
                    Assert.IsTrue(csvFile.GetValueAsString(CsvSqlConstants.DateOfBirth) == "1992-08-05");
                }
            }
        }
    }
}
