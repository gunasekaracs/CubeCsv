using System.IO;
using System.Threading.Tasks;

namespace CubeCsv
{
    class CsvHashGenerator
    {
        private string _hashColumnName;
        private CsvFile _tableDirect;
        private CsvConfiguration _configuration;

        public CsvHashGenerator(string hashColumnName, CsvFile tableDirect, CsvConfiguration configuration)
        {
            _hashColumnName = hashColumnName;
            _tableDirect = tableDirect;
            _configuration = configuration;
        }
        public async Task<CsvFile> GenerateHashAsync()
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            StreamReader reader = new StreamReader(stream);
            for (int i = 0; i < _configuration.SkipRowCount; i++)
                writer.WriteLine(string.Empty);
            _configuration.Schema.Add(
                new CsvFieldSchema()
                {
                    Name = _hashColumnName,
                    Type = typeof(string),
                });
            while (await _tableDirect.ReadAsync())
            {
                _tableDirect.Current.AddHashColumn();
                writer.WriteLine(_tableDirect.Current.ToString(_configuration.Delimiter));
            }
            writer.Flush();
            _tableDirect.Dispose();
            return new CsvFile(reader, _configuration);
        }
    }
}
