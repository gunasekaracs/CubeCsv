using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CubeCsv.Exceptions;

namespace CubeCsv
{
    public class CsvRow : List<CsvField>
    {
        private CsvHeader _header;

        public CsvHeader Header
        {
            get { return _header; }
            set { _header = value; }
        }

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

        public string ToSql()
        {
            StringBuilder builder = new StringBuilder("(");
            if (Count != _header.Count)
                throw new CsvHeaderCountMismatchException("Header count and field count does not match");
            for (int i = 0; i < Count; i++)
            {
                CsvFieldHeader header = _header[i];
                Type type = header.Schema.Type;
                if (type == typeof(int) || type == typeof(float) || type == typeof(double))
                    builder.Append(this[i].Value);
                else
                    builder.Append($"'{ this[i].Value }'");
                builder.Append(",");
            }
            builder.Append(")");
            return builder.ToString().Replace(",)", ")");
        }
    }
}
