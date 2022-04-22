using System.IO;

namespace CubeCsv
{
    public abstract class CsvBase
    {
        protected StreamReader _reader;
        protected CsvConfiguration _configuration;
        protected CsvHeader _header;

        public CsvHeader Header { get { return _header; } }
        public CsvBase(CsvConfiguration configuration, StreamReader reader)
        {
            _configuration = configuration;
            _reader = reader;
            _header = new CsvHeader(_configuration, _reader);
            _header.ResolveSchema(_configuration.Delimiter);
        }
    }
}
