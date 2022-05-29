using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Threading.Tasks;

namespace CubeCsv
{
    class CsvSqlReader : CsvSqlBase
    {
        private string _whereClause { get; set; }

        public CsvSqlReader(string table, SqlConnection connection, string whereClause, CsvConfiguration configuration)
            : base(table, connection, configuration)
        {
            _whereClause = whereClause;
        }

        public async Task<TableDirect> ReadRowsFromTableAsync()
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            if (_schema == null) _schema = await GetSchemaAsync();
            CsvHeader header = _schema.ToHeader();
            if (_connection.State == ConnectionState.Closed)
                await _connection.OpenAsync();
            using (SqlCommand command = new SqlCommand(_queryBuilder.GetSelectStatement(_table, _whereClause), _connection))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        CsvRow row = new CsvRow();
                        foreach (var column in header)
                            row.Add(new CsvField() { Ordinal = column.Ordinal, Value = reader.GetValue(reader.GetOrdinal(column.Schema.Name)) });
                        writer.WriteLine(row);
                    }
                    writer.Flush();
                    writer.Close();
                    _configuration.Schema = _schema;
                    return new TableDirect(new StreamReader(stream), _configuration);
                }
            }
        }
    }
}
