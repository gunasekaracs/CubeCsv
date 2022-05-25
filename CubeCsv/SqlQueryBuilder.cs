using System;
using CubeCsv.Exceptions;

namespace CubeCsv
{
    class SqlQueryBuilder : ISqlQueryBuilder
    {
        public string GetTableExistsSql(string table)
        {
            return $@"IF (EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = '{ table }'))
                BEGIN
	                SELECT 'true';
                END
                ELSE
                BEGIN
	                SELECT 'false'
                END;";
        }
        public string GetSchemaReadingSql(string table)
        {
            return $"SELECT * FROM \"{ table }\" WHERE 1 = 2";
        }
        public string GetSelectStatement(string table, string where)
        {
            if (!string.IsNullOrEmpty(where))
            {
                where = where.Trim();
                if (!where.StartsWith("WHERE", StringComparison.OrdinalIgnoreCase))
                    where = $"WHERE { where }";
            }
            if (string.IsNullOrEmpty(table))
                throw new CsvInvalidSqlException("You have to provide a table name in order to read from sql table into csv");
            return $"SELECT * FROM \"{ table }\" { where }".Trim();
        }
    }
}
