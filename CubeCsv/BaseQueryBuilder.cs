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
            return $@"SELECT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = '{table}');";
        }
        public virtual string GetSchemaReadingSql(string table)
        {
            return $"SELECT * FROM \"{table}\" WHERE 1 = 2";
        }
        public virtual string GetSelectStatement(string table, string where)
        {
            if (!string.IsNullOrEmpty(where))
            {
                where = where.Trim();
                if (!where.StartsWith("WHERE", StringComparison.OrdinalIgnoreCase))
                    where = $"WHERE {where}";
            }
            if (string.IsNullOrEmpty(table))
                throw new CsvInvalidSqlException("You have to provide a table name in order to read from sql table into csv");
            return $"SELECT * FROM \"{table}\" {where}".Trim();
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
                schema.Length = schema.Length = int.Parse(row[4].ToString());
            return schema;
        }
    }
}
