using System.IO;
using System.Threading.Tasks;
using CubeCsv.Exceptions;

namespace CubeCsv
{
    class CsvColumnManager
    {
        public async Task<CsvFile> AddColumnWithValue(CsvFile tableDirect, CsvConfiguration configuration, int location, CsvFieldSchema schema, object value)
        {
            if (value == null)
                throw new CsvNullValueException("Value cannot be null");
            if (location > tableDirect.Header.Count)
                throw new CsvOutBoundException("Location is out of bounds");
            if (schema == null)
                throw new CsvNullSchemaException("Schema cannot be null");
            CsvFieldHeader header = new CsvFieldHeader();
            header.Ordinal = location;
            header.Schema = schema;
            if (value.GetType() != schema.Type)
                throw new CsvInvalidCastException($"Schema type {header.Schema.Type}and value type [{value.GetType()}] does not match");
            MemoryStream stream = new MemoryStream();
            StreamReader reader = new StreamReader(stream);
            StreamWriter writer = new StreamWriter(stream);
            for (int i = 0; i < configuration.SkipRowCount; i++)
                writer.WriteLine(string.Empty);
            while (await tableDirect.ReadAsync())
            {
                tableDirect.Current.Insert(location, new CsvField() { Value = value });
                writer.WriteLine(tableDirect.Current.ToString(configuration.Delimiter));
            }
            writer.Flush();
            tableDirect.AddHeader(location, header);
            configuration.Schema = tableDirect.Header.ToSchema();
            tableDirect.Dispose();
            return new CsvFile(reader, configuration);
        }
    }
}
