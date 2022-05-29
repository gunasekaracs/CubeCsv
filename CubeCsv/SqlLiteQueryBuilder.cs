using System;
using System.Data;

namespace CubeCsv
{
    class SqlLiteQueryBuilder : BaseQueryBuilder, ISqlQueryBuilder
    {
        public override string GetTableExistsSql(string table)
        {
            return $"SELECT EXISTS(SELECT name FROM sqlite_master WHERE type='table' AND name='{table}');";
        }
        public override CsvFieldSchema GetFieldSchema(DataRow row)
        {
            var schema = new CsvFieldSchema()
            {
                Name = row[0].ToString(),                
                Type = Type.GetType(row[12].ToString())
            };
            return schema;
        }
    }
}
