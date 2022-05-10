using System.Globalization;

namespace CubeCsv
{
    public sealed class CsvConfiguration
    {
        public bool HasHeader { get; set; } = true;
        public CultureInfo CultureInfo { get; set; } = CultureInfo.InvariantCulture;       
        public string Delimiter { get; set; } = ",";
        public bool HeaderDoubleQuoted { get; set; } = true;
    }
}
