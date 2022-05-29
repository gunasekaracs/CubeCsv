using CubeCsv.Exceptions;
using Microsoft.Data.Sqlite;
using System;
using System.Data.Common;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace CubeCsv
{
    class CsvSqlCommand : IDisposable
    {
        private DbCommand command;
        private ISqlQueryBuilder queryBuilder;
        public ISqlQueryBuilder QueryBuilder => queryBuilder;
        public CsvSqlCommand(string sql, DbConnection connection)
        {
            if (connection is SqlConnection sqlConnection)
            {
                queryBuilder = new TsqlQueryBuilder();
                command = new SqlCommand(sql, sqlConnection);
            }
            else if (connection is SqliteConnection sqliteConnection)
                command = new SqliteCommand(sql, sqliteConnection);
            else
                throw new CsvInvalidConnectionException("Unrecognized connection type");
        }
        public async Task<object> ExecuteScalarAsync()
        {
            return await command.ExecuteScalarAsync();
        }
        public async Task<DbDataReader> ExecuteReaderAsync()
        {
            return await command.ExecuteReaderAsync();
        }
        public async Task<int> ExecuteNonQueryAsync()
        {
            return await command.ExecuteNonQueryAsync();
        }
        public void Dispose()
        {
            command?.Dispose();
        }
    }
}
