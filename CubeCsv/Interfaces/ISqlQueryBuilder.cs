namespace CubeCsv
{
    public interface ISqlQueryBuilder
    {
        string GetTableExistsSql(string table);
        string GetSchemaReadingSql(string table);
        string GetSelectStatement(string table, string where);
    }
}
