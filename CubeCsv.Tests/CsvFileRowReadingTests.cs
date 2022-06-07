using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Globalization;
using System.IO;
using System.Text;

namespace CubeCsv.Tests
{
    [TestClass]
    public class CsvFileRowReadingTests
    {
        [TestMethod]
        public void CsvFileRowReadingTest()
        {
            ASCIIEncoding encoding = new ASCIIEncoding();
            var data = encoding.GetBytes(CsvFiles.CsvFileRowReading);
            using (var stream = new MemoryStream(data))
            {
                using (var streamReader = new StreamReader(stream))
                using (var csvFile = new CsvFile(streamReader, new CsvConfiguration()
                {
                    CultureInfo = CultureInfo.InvariantCulture,
                    Delimiter = ";",
                    HasHeader = false
                }))
                {
                    Assert.IsTrue(csvFile.CountAsync().Result == 9);
                }
            }
        }
    }
}
