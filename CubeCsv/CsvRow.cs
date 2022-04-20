using System.Collections.Generic;
using System.Linq;

namespace CubeCsv
{
    public class CsvRow : List<CsvField>
    {
        private CsvHeader _header;
        public CsvRow(CsvHeader header)
        {
            _header = header;
        }
        public CsvField this[string name]
        {
            get
            {
                CsvFieldHeader header = _header[name];
                return this.FirstOrDefault(x => x.Ordinal == header.Ordinal);
            }
        }
    }
}
