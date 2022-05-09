using Microsoft.Data.Sqlite;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CubeCsv.Tests
{
    [TestClass]
    public class CsvSchemaReadingTests
    {
        [TestMethod]
        public void CsvSchemaReadingTest()
        {
            CsvDbContext context = new CsvDbContext();
            context.Seed();

            using SqliteConnection connection = context.CreateConnection();

            CsvSqlWriter csvSqlWriter = new (CsvSqlConstants.EmployeesTableName, null, new CsvRows());
        }
    }
}
