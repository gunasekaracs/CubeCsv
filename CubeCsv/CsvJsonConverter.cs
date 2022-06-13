using System.IO;
using System.Threading.Tasks;

namespace CubeCsv
{
    class CsvJsonConverter
    {
        private CsvConfiguration _configuration;
        private CsvFile _tableDirect;
        private string _columnName;

        public CsvJsonConverter(string columnName, CsvFile tableDirect, CsvConfiguration configuration)
        {
            _columnName = columnName;
            _configuration = configuration;
            _tableDirect = tableDirect;
        }
        public async Task<CsvFile> ConvertToJsonRowCollectionAsync()
        {
            var schema = new CsvSchema();
            schema.Add(new CsvFieldSchema(_columnName, typeof(string)));
            _configuration.RemoveLineBreaks = true;
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            StreamReader reader = new StreamReader(stream);
            for (int i = 0; i < _configuration.SkipRowCount; i++)
                writer.WriteLine(string.Empty);
            var header = _configuration.Schema.ToHeader();
            while (await _tableDirect.ReadAsync())
            {
                var row = new CsvRow();
                row.Add(new CsvField() { Value = _tableDirect.Current.ToJson(header) });
                writer.WriteLine(row.ToString(_configuration.Delimiter));
            }
            writer.Flush();
            _configuration.Schema = schema;
            return new CsvFile(reader, _configuration);
        }
    }
}
