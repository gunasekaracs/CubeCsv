using System;
using System.Collections.Generic;

namespace CubeCsv
{
    public class CsvSchema : List<CsvFieldSchema>
    {
        internal CsvHeader ToHeader()
        {
            CsvHeader header = new CsvHeader();
            foreach(CsvFieldSchema field in this)
                header.Add(new CsvFieldHeader()
                {
                    Ordinal = this.IndexOf(field),
                    Schema = new CsvFieldSchema()
                    {
                        Name = field.Name,
                        Type = field.Type,
                        Length = field.Length
                    }
                });
            return header;
        }
    }
}
