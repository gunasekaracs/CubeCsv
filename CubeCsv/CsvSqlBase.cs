using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using CubeCsv.Exceptions;

namespace CubeCsv
{
    public class CsvSqlBase
    {
        protected string _table;
        protected SqlConnection _connection;
        protected List<string> _columnExlusions;
        protected CsvSchema _schema;
        protected ISqlQueryBuilder _queryBuilder = new SqlQueryBuilder();

        public CsvSqlBase(string table, SqlConnection connection, CsvSchema schema = null, List<string> columnExlusions = null)
        {
            _table = table;
            _connection = connection;
            _schema = schema;
            _columnExlusions = columnExlusions ?? new List<string>();
            if (_connection == null)
                throw new CsvNullConnectionException("You have to specify a not null sql connection");
        }
        public async Task<CsvSchema> GetSchemaAsync()
        {
            if (_connection.State == ConnectionState.Closed)
                await _connection.OpenAsync();
            
            string sql = _queryBuilder.GetTableExistsSql(_table);;
            SqlCommand command = new SqlCommand(sql, _connection);
            if (bool.TryParse((await command.ExecuteScalarAsync() ?? string.Empty).ToString(), out var exists) && exists)
            {
                command = new SqlCommand(_queryBuilder.GetSchemaReadingSql(_table), _connection);
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
