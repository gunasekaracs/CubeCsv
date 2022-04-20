using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace CubeCsv
{
    public class CsvFile : CsvBase, IDisposable
    {
        public CsvFile(StreamReader reader, CsvConfiguration configuration) : base(configuration, reader) { }
        public CsvFile(StreamReader reader, CultureInfo cultureInfo) : base(new CsvConfiguration() { CultureInfo = cultureInfo }, reader) { }
        public CsvValidationResult Validate(CsvSchema schema)
        {
            return Validate(schema.ToArray());
        }
        public CsvValidationResult Validate(params CsvFieldSchema[] fields)
        {
            var result = new CsvValidationResult();
            if (Header.Count != fields.Length)
            {
                result.Errors.Add("Column count does not match");
                return result;
            }
            var list = new List<CsvFieldSchema>(fields);
            foreach (var field in fields)
            {
                int index = list.IndexOf(field);
                if (!field.Equals(Header[index].Schema))
                    result.Errors.Add($"Column at the index { index } is a type mismatch");
            }
            return result;
        }
        public void Dispose()
        {
            _header.Dispose();
            _header = null;
        }
    }
}
