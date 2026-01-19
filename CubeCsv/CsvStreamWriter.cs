using System;
using System.IO;
using System.Threading.Tasks;

namespace CubeCsv
{
    public class CsvStreamWriter: IDisposable
    {
        private readonly CsvFile _csvFile;
        private readonly StreamWriter _writer;
        private CsvConfiguration _configuration;

        public CsvStreamWriter(CsvFile csvFile, StreamWriter writer, CsvConfiguration configuration)
        {
            _csvFile = csvFile;
            _writer = writer;
            _configuration = configuration;
        }

        public async Task WriteToStreamAsync()
        {
            _writer.WriteLine(_csvFile.Header.ToString(_configuration.Delimiter));
            while (await _csvFile.ReadAsync())
            {
                _writer.WriteLine(_csvFile.Current.ToString(','));
            }
        }

        public void Dispose()
        {
            _csvFile.Dispose();
            _writer.Dispose();
            _configuration = null;
        }
    }
}
