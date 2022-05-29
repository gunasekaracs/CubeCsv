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
                    Ordinal = IndexOf(field),
                    Schema = field
                });
            return header;
        }
    }
}
