using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;

namespace CubeCsv
{
    class CsvSqlWriter : CsvSqlBase
    {
        private TableDirect _tableDirect;

        public CsvSqlWriter(string table, SqlConnection connection, TableDirect tableDirect, CsvConfiguration configuration)
            : base(table, connection, configuration)
        {
            _tableDirect = tableDirect;
        }
        public async Task<int> WirteRowsToTableAsync()
        {
            int count = 0;
            foreach (var sql in await GenerateSqlAsync())
            {
                if (_connection.State == ConnectionState.Closed)
                    await _connection.OpenAsync();
                SqlCommand command = new SqlCommand(sql, _connection);
                count += command.ExecuteNonQuery();
            }
            _connection.Close();
            return count;
        }
        public async Task<List<string>> GenerateSqlAsync()
        {
            List<string> sql = new List<string>();
            int count = 1;
            string script;
            StringBuilder builder = await CreateSqlBuilderAsync();
            while (await _tableDirect.ReadAsync())
            {
                CsvRow row = _tableDirect.Current;

                builder.AppendLine($"{row.ToSql(_tableDirect.Header)},");
                if (count % _configuration.SqlRowBatchSize == 0)
                {
                    script = builder.ToString().Trim();
                    sql.Add(script.Remove(script.Length - 1, 1));
                    builder = await CreateSqlBuilderAsync();
                }
                count++;
            }
            if (builder.Length > 0)
            {
                script = builder.ToString().Trim();
                sql.Add(script.Remove(script.Length - 1, 1));
            }
            return sql;
        }
        private async Task<StringBuilder> CreateSqlBuilderAsync()
        {
            if (_schema == null) _schema = await GetSchemaAsync();
            return new StringBuilder();
        }
    }
}
