using CubeCsv.Exceptions;
using System.Collections.Generic;

namespace CubeCsv
{
    class CsvValidator
    {
        public CsvValidationResult ValidateSchema(CsvFile csvFile, params CsvFieldSchema[] fields)
        {
            var result = new CsvValidationResult();
            if (csvFile.Header.Count != fields.Length)
            {
                result.Errors.Add(new CsvRowReadError() { Error = "Column count does not match" });
                return result;
            }
            var list = new List<CsvFieldSchema>(fields);
            foreach (var field in fields)
            {
                int index = list.IndexOf(field);
                if (!field.Equals(csvFile.Header[index].Schema))
                    result.Errors.Add(new CsvRowReadError() { Error = $"Column at the index {index} is a type mismatch" });
            }
            return result;
        }
    }
}
