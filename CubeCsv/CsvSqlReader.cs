using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace CubeCsv
{
    public class CsvSqlReader : CsvSqlBase
    {
        private string _whereClause { get; set; }

        public CsvSqlReader(string table, SqlConnection connection, string whereClause, CsvSchema schema = null, List<string> columnExlusions = null)
            : base(table, connection, schema, columnExlusions)
        {
            _whereClause = whereClause;
        }

        public async Task<CsvFile> ReadRowsFromTableAsync()
        {
            CsvRows rows = new CsvRows();
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
                        CsvRow row = new CsvRow(header, true);
                        foreach (var column in header)
                            row.Add(new CsvField() { Ordinal = column.Ordinal, Value = reader.GetValue(reader.GetOrdinal(column.Schema.Name)) });
                        rows.Add(row);
                    }
                    return new CsvFile(rows, header);
                }
            }
        }
    }
}
