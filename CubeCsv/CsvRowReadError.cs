namespace CubeCsv
{
    public class CsvRowReadError
    {
        public enum Type
        {
            Row,
            Header,
            Field
        }
        public int RowNumber { get; set; }
        public string Error { get; set; }        
    }
}
