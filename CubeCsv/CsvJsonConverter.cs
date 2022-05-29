using System.IO;
using System.Threading.Tasks;

namespace CubeCsv
{
    class CsvJsonConverter
    {
        private CsvConfiguration _configuration;
        private TableDirect _tableDirect;
        private string _columnName;

        public CsvJsonConverter(string columnName, TableDirect tableDirect, CsvConfiguration configuration)
        {
            _columnName = columnName;
            _configuration = configuration;
            _tableDirect = tableDirect;
        }
        public async Task<TableDirect> ConvertToJsonRowCollectionAsync()
        {
            var schema = new CsvSchema();
            schema.Add(new CsvFieldSchema(_columnName, typeof(string)));
            _configuration.RemoveLineBreaks = true;
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            StreamReader reader = new StreamReader(stream);
            while (await _tableDirect.ReadAsync())
            {
                var row = new CsvRow();
                row.Add(new CsvField() { Value = _tableDirect.Current.ToJson(_configuration.Schema.ToHeader()) });
                writer.WriteLine(row.ToString(_configuration.Delimiter));
            }
            writer.Flush();
            _configuration.Schema = schema;
            return new TableDirect(reader, _configuration);
        }
    }
}
