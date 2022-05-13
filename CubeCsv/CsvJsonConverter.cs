namespace CubeCsv
{
    public class CsvJsonConverter
    {
        private CsvSchema _schema;
        private CsvRows _rows;
        private string _columnName;

        public CsvJsonConverter(string columnName, CsvRows rows, CsvSchema schema)
        {
            _columnName = columnName;
            _schema = schema;
            _rows = rows;
        }
        public (CsvRows Rows, CsvSchema Schema) ConvertToJsonRowCollection()
        {
            var rows = new CsvRows();
            var schema = new CsvSchema();
            schema.Add(new CsvFieldSchema(_columnName, typeof(string)));
            foreach (CsvRow row in _rows)
            {
                row.Header = _schema.ToHeader();
                var jsonRow = new CsvRow(schema.ToHeader(), false);
                jsonRow.Add(new CsvField() { Value = row.ToJson() });
                rows.Add(jsonRow);
            }
            return (rows, schema);
        }
    }
}
