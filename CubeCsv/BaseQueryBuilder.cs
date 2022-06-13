using CubeCsv.Exceptions;
using System;
using System.Data;
using System.Linq;

namespace CubeCsv
{
    class BaseQueryBuilder
    {
        public virtual string GetTableExistsSql(string table)
        {
            return $@"IF (EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = '{table}'))
                BEGIN
	                SELECT 1;
                END
                ELSE
                BEGIN
	                SELECT 0
                END;";
        }
        public virtual string GetSchemaReadingSql(string table)
        {
            return $"SELECT * FROM \"{table}\" WHERE 1 = 2";
        }
        public virtual string GetSelectStatement(string table, string where, string orderBy)
        {
            where = Sanatize("WHERE", where);
            orderBy = Sanatize("ORDER BY", orderBy);
            if (string.IsNullOrEmpty(table))
                throw new CsvInvalidSqlException("You have to provide a table name in order to read from sql table");
            return $"SELECT * FROM \"{table}\"{where}{orderBy}".Trim();
        }
        public virtual string GetInsertString(CsvSchema schema, string table)
        {
            return $"INSERT INTO \"{table}\" ({string.Join(",", schema.Select(x => x.Name).ToArray())}) VALUES {Environment.NewLine}";
        }
        public virtual CsvFieldSchema GetFieldSchema(DataRow row)
        {
            var schema = new CsvFieldSchema()
            {
                Name = row[0].ToString(),
                Type = Type.GetType(row[12].ToString())
            };
            if (schema.Type == typeof(string))
                schema.Length = schema.Length = int.Parse(row[2].ToString());
            return schema;
        }

        private string Sanatize(string operation, string condition)
        {
            if (string.IsNullOrEmpty(condition)) return condition;
            condition = condition.Trim();
            if (!condition.StartsWith(operation, StringComparison.OrdinalIgnoreCase))
                condition = $" {operation} {condition}";
            if (!condition.StartsWith(" "))
                condition = $" {condition}";
            return condition;
        }
    }
}
