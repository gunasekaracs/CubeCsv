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
        }
    }
}
