using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Threading.Tasks;
using CubeCsv.Exceptions;
using Microsoft.Data.Sqlite;

namespace CubeCsv
{
    class CsvSqlBase
    {
        protected string _table;
        protected DbConnection _connection;
        protected CsvConfiguration _configuration;
        protected CsvSchema _schema;
        protected ISqlQueryBuilder _queryBuilder;

        public CsvSqlBase(string table, DbConnection connection, CsvConfiguration configuration)
        {
            _connection = connection;
            _table = table;
            _configuration = configuration;
            _schema = configuration.Schema;
            if (_connection == null)
                throw new CsvNullConnectionException("You have to specify a not null sql connection");
            if (_connection is SqlConnection)
                _queryBuilder = new TsqlQueryBuilder();
            else if (_connection is SqliteConnection)
                _queryBuilder = new SqlLiteQueryBuilder();
            else
                throw new CsvInvalidConnectionException("Unrecognized connection type");
        }
        public async Task<CsvSchema> GetSchemaAsync()
        {
            if (_connection.State == ConnectionState.Closed)
                await _connection.OpenAsync();

            string sql = _queryBuilder.GetTableExistsSql(_table);
            CsvSqlCommand command = new CsvSqlCommand(sql, _connection);
            
            if (int.TryParse((await command.ExecuteScalarAsync() ?? string.Empty).ToString(), out var exists) && exists == 1)
            {
                command = new CsvSqlCommand(_queryBuilder.GetSchemaReadingSql(_table), _connection);
                var reader = await command.ExecuteReaderAsync();
                var schemaTable = reader.GetSchemaTable();
                CsvSchema schema = new CsvSchema();
                foreach (DataRow row in schemaTable.Rows)
                {
                    var fieldSchema = _queryBuilder.GetFieldSchema(row);
                    if (_configuration.ColumnExlusions.Exists(x => x == fieldSchema.Name)) continue;
                    schema.Add(fieldSchema);
                }
                _connection.Close();
                return schema;
            }
            else
                throw new CsvMissingTableException($"Table {_table} does not exists");
        }
    }
}
