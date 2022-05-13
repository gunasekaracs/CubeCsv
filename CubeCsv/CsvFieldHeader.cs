namespace CubeCsv
{
    public class CsvFieldHeader: CsvOrdinal
    {
        public CsvFieldSchema Schema { get; set; }
        public CsvFieldHeader()
        {
            Schema = new CsvFieldSchema();
        }
    }
}
