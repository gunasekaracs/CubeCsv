using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CubeCsv
{
    public class CsvSqlWriter : CsvSqlBase
    {
        private CsvRows _rows;
        private int _sqlRowBatchSize;

        public CsvSqlWriter(string table, SqlConnection connection, CsvRows rows, int SqlRowBatchSize, CsvSchema schema = null, List<string> columnExlusions = null)
            : base(table, connection, schema, columnExlusions)
        {
            _rows = rows;
            _sqlRowBatchSize = SqlRowBatchSize;
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
            return _rows.Count;
        }
        public async Task<List<string>> GenerateSqlAsync()
        {
            List<string> sql = new List<string>();
            int count = 1;
            string script;
            StringBuilder builder = await CreateSqlBuilderAsync();
            foreach (var row in _rows)
            {
                if (!row.HasHeader)
                    row.Header = _schema.ToHeader();
                builder.AppendLine($"{ row.ToSql() },");
                if (count % _sqlRowBatchSize == 0)
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
            return new StringBuilder($"INSERT INTO \"{ _table }\" ({ string.Join(",", _schema.Select(x => x.Name).ToArray()) }) VALUES { Environment.NewLine }");
        }
    }
}
