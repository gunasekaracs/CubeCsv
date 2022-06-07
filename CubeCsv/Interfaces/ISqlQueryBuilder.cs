using System.Data;

namespace CubeCsv
{
    public interface ISqlQueryBuilder
    {
        string GetTableExistsSql(string table);
        string GetSchemaReadingSql(string table);
        string GetSelectStatement(string table, string where, string orderBy);
        string GetInsertString(CsvSchema schema, string table);
        CsvFieldSchema GetFieldSchema(DataRow row);
    }
}
