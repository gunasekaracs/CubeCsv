namespace CubeCsv
{
    public class CsvFieldValidator
    {
        public struct ValidatorType
        {
            public const string Regex = "Regex";
        }
        public string Type { get; set; }
        public string RegularExpression { get; set; }
        public string Description { get; set; }
    }
}
