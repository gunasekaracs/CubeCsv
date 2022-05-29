using System.IO;
using System.Threading.Tasks;

namespace CubeCsv
{
    class CsvJsonConverter
    {
        private CsvSchema _schema;
        private TableDirect _tableDirect;
        private string _columnName;

        public CsvJsonConverter(string columnName, TableDirect tableDirect, CsvSchema schema)
        {
            _columnName = columnName;
            _schema = schema;
            _tableDirect = tableDirect;
        }
        public async Task<TableDirect> ConvertToJsonRowCollectionAsync()
        {
            var schema = new CsvSchema();
            schema.Add(new CsvFieldSchema(_columnName, typeof(string)));
            CsvConfiguration configuration = new CsvConfiguration() { Schema = schema };
            configuration.RemoveLineBreaks = true;
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            while(await _tableDirect.ReadAsync())
            {                
                var row = new CsvRow();
                row.Add(new CsvField() { Value = _tableDirect.Current.ToJson(_tableDirect.Header) });
                writer.WriteLine(row);
            }
            return new TableDirect(new StreamReader(stream), configuration);
        }
    }
}
