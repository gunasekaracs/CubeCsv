using System.IO;
using System.Threading.Tasks;

namespace CubeCsv
{
    class CsvHashGenerator
    {
        private string _hashColumnName;
        private TableDirect _tableDirect;
        private CsvConfiguration _configuration;

        public CsvHashGenerator(string hashColumnName, TableDirect tableDirect, CsvConfiguration configuration)
        {
            _hashColumnName = hashColumnName;
            _tableDirect = tableDirect;
            _configuration = configuration;
        }
        public async Task<TableDirect> GenerateHashAsync()
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            StreamReader reader = new StreamReader(stream);
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
            return new TableDirect(reader, _configuration);
        }
    }
}
