using System.Collections.Generic;
using System.Threading.Tasks;

namespace CubeCsv
{
    class CsvValidator
    {
        public CsvValidationResult ValidateSchema(CsvFile csvFile, params CsvFieldSchema[] fields)
        {
            var result = new CsvValidationResult();
            if (csvFile.Header.Count != fields.Length)
            {
                result.Errors.Add("Column count does not match");
                return result;
            }
            var list = new List<CsvFieldSchema>(fields);
            foreach (var field in fields)
            {
                int index = list.IndexOf(field);
                if (!field.Equals(csvFile.Header[index].Schema))
                    result.Errors.Add($"Column at the index {index} is a type mismatch");
            }
            return result;
        }
        public async Task<CsvValidationResult> ValidateAsync(CsvFile csvFile)
        {
            CsvValidationResult result = new CsvValidationResult();
            var header = csvFile.Header;
            while (await csvFile.ReadAsync())
            {
                string error = $"Error at the row {csvFile.Location}";
                CsvRow row = csvFile.Current;
                if (row.Count != header.Count)
                    result.Errors.Add($"{error}, Header count and field count does not match. Row has {row.Count} columns and header has {header.Count}. Row = [{row}] and Header = [{header}]");
                for (int i = 0; i < header.Count; i++)
                {
                    CsvField field = row[i];
                    CsvFieldHeader column = header[i];
                    object value = field.Value;
                    if (column != null && column.Schema.Type != value.GetType())
                        result.Errors.Add($"{error}, schema type [{column.Schema.Type}] and value type [{value.GetType()}] does not match");
                }
            }
            return result;
        }
    }
}
