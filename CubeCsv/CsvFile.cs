using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;

namespace CubeCsv
{
    public class CsvFile : CsvBase, IDisposable
    {
        public CsvRows Rows { get; set; } = new CsvRows();
        public int Count { get { return Rows.Count; } }
        public bool Encrypted { get; private set; }

        public CsvFile() { }
        public CsvFile(CsvRows rows, CsvHeader header)
        {
            Rows = rows;
            _header = header;
        }
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
        public async Task ReadAllRowsAsync()
        {
            Rows = new CsvRows();
            CsvReader reader = new CsvReader(_reader, _configuration);
            while (await reader.ReadAsync())
                if (reader.Current != null)
                    Rows.Add(reader.Current);

        }
        public void Dispose()
        {
            _header.Dispose();
            _header = null;
        }
        public async Task<CsvFile> ConvertToJsonAsync(string jsonColumnName)
        {            
            return await Task.Run(() =>
            {
                var jsonConverter = new CsvJsonConverter(jsonColumnName, Rows, Header.ToSchema());
                var result = jsonConverter.ConvertToJsonRowCollection();
                return new CsvFile(result.Rows, result.Schema.ToHeader());
            });
        }
        public async Task<CsvFile> AddHashColumnColumn(string hashColumnName)
        {
            return await Task.Run(() =>
            {
                foreach (var row in Rows)
                    row.AddHashColumn(hashColumnName);
                return this;
            });
        }
        public async Task<CsvFile> EncryptData(string key, string[] columnExclusions)
        {
            Encrypted = true;
            return await Task.Run(() =>
            {
                foreach (var row in Rows)
                    row.Encrypt(key, columnExclusions);
                return this;
            });
        }
        public async Task<CsvFile> DecryptData(string key, string[] columnExclusions)
        {
            Encrypted = true;
            return await Task.Run(() =>
            {
                foreach (var row in Rows)
                    row.Decrypt(key, columnExclusions);
                return this;
            });
        }
        public async Task<int> WirteRowsToTableAsync(string table, SqlConnection connection, CsvSchema schema = null, List<string> columnExlusions = null) =>
            await new CsvSqlWriter(table, connection, Rows, _configuration.SqlRowBatchSize, schema, columnExlusions).WirteRowsToTableAsync();
        public void AddHeader(int location, CsvFieldHeader header)
        {
            Header.Insert(location, header);
            foreach (CsvRow row in Rows)
                row.Header = Header;
        }
    }
}
