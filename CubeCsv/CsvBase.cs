using System.IO;

namespace CubeCsv
{
    public abstract class CsvBase
    {
        protected StreamReader _reader;
        protected CsvConfiguration _configuration = new CsvConfiguration();
        protected CsvHeader _header;
        protected int _skipRowCount = 1;

        public CsvHeader Header { get { return _header; } }
        public CsvConfiguration Configuration
        {
            get { return _configuration; }
            set { _configuration = value; }
        }

        public CsvBase()
        {
            _header = new CsvHeader();
        }
        public CsvBase(CsvConfiguration configuration, StreamReader reader)
        {
            _configuration = configuration;
            _reader = reader;
            _header = new CsvHeader(_configuration, _reader);            
            _header.ResolveSchema(_configuration.Delimiter);
            _skipRowCount = _configuration.SkipRowCount;    
        }
    }
}
