using System.Collections.Generic;
using System.Globalization;
using CubeCsv.Processors;

namespace CubeCsv
{
    public sealed class CsvConfiguration
    {
        public bool HasHeader { get; set; }
        public CultureInfo CultureInfo { get; set; } = CultureInfo.InvariantCulture;       
        public char Delimiter { get; set; } = ',';
        public bool HeaderDoubleQuoted { get; set; } = true;
        public bool BreakOnError { get; set; } = true;
        public int SqlRowBatchSize { get; set; } = 1000;
        public int SkipRowCount { get; set; } = 0;
        public bool IncludeDataInLogs { get; set; } = true;
        public bool RemoveLineBreaks { get; set; }
        public CsvSchema Schema { get; set; }
        public ICsvCellCleaner CellCleaner { get; set; }
        public ICsvRowCleaner RowCleaner { get; set; }
        public List<string> ColumnExlusions { get; set; } = new List<string>();
    }
}
