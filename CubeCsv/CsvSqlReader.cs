using System.Data;
using System.Data.Common;
using System.IO;
using System.Threading.Tasks;

namespace CubeCsv
{
    class CsvSqlReader : CsvSqlBase
    {
        private string _whereClause { get; set; }

        public CsvSqlReader(string table, DbConnection connection, string whereClause, CsvConfiguration configuration)
            : base(table, connection, configuration)
        {
            _whereClause = whereClause;
        }

        public async Task<TableDirect> ReadRowsFromTableAsync()
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            StreamReader reader = new StreamReader(stream);
            if (_schema == null) _schema = await GetSchemaAsync();
            CsvHeader header = _schema.ToHeader();
            if (_connection.State == ConnectionState.Closed)
                await _connection.OpenAsync();
            using (CsvSqlCommand command = new CsvSqlCommand(_queryBuilder.GetSelectStatement(_table, _whereClause), _connection))
            {
                using (DbDataReader dataReader = await command.ExecuteReaderAsync())
                {
                    while (dataReader.Read())
                    {
                        CsvRow row = new CsvRow();
                        foreach (var column in header)
                            row.Add(new CsvField() { Ordinal = column.Ordinal, Value = dataReader.GetValue(dataReader.GetOrdinal(column.Schema.Name)) });
                        writer.WriteLine(row);
                    }                   
                    _configuration.Schema = _schema;
                    writer.Flush();
                    stream.Seek(0, SeekOrigin.Begin);
                }
                _connection.Close();
                return new TableDirect(reader, _configuration);
            }
        }
    }
}
