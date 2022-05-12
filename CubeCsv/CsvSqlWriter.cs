using CubeCsv.Exceptions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CubeCsv
{
    public class CsvSqlWriter
    {
        private string _table;
        private SqlConnection _connection;
        private CsvRows _rows;
        private List<string> _columnExlusions;
        private CsvSchema _schema;
        private int _sqlRowBatchSize;

        public CsvSqlWriter(string table, SqlConnection connection, CsvRows rows, int SqlRowBatchSize, List<string> columnExlusions = null)
        {
            _table = table;
            _connection = connection;
            _rows = rows;
            _sqlRowBatchSize = SqlRowBatchSize;
            _columnExlusions = columnExlusions ?? new List<string>();
            if (_connection == null)
                throw new CsvNullConnectionException("You have to specify a not null sql connection");
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

        public async Task<CsvSchema> GetSchemaAsync()
        {
            if (_connection.State == ConnectionState.Closed)
                await _connection.OpenAsync();

            string sql =
                $@"IF (EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = '{ _table }'))
                BEGIN
	                SELECT 'true';
                END
                ELSE
                BEGIN
	                SELECT 'false'
                END;";

            SqlCommand command = new SqlCommand(sql, _connection);
            if (bool.TryParse((await command.ExecuteScalarAsync() ?? string.Empty).ToString(), out var exists) && exists)
            {
                command = new SqlCommand($"SELECT * FROM \"{ _table }\" WHERE 1 = 2", _connection);
                var reader = command.ExecuteReader();
                var schemaTable = reader.GetSchemaTable();
                CsvSchema schema = new CsvSchema();
                foreach (DataRow row in schemaTable.Rows)
                {
                    string name = row[0].ToString();
                    if (_columnExlusions.Exists(x => x == name)) continue;
                    schema.Add(new CsvFieldSchema()
                    {
                        Name = name,
                        Length = int.Parse(row[4].ToString()),
                        Type = Type.GetType(row[12].ToString())
                    });
                }
                _connection.Close();
                return schema;
            }
            else
                throw new CsvMissingTableException($"Table { _table } does not exists");
        }
    }
}
