using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;

namespace CubeCsv
{
    public class TableDirect : IDisposable
    {
        private CsvStreamReader _csvStreamReader;
        private CsvSqlWriter _csvSqlWriter;
        private CsvSqlReader _csvSqlReader;
        private int _count;
        public bool Encrypted { get; private set; }

        public CsvRow Current => _csvStreamReader.Current;
        public CsvHeader Header => _csvStreamReader.Header;
        public int Location => _csvStreamReader.Location;

        public TableDirect(StreamReader reader, CsvConfiguration configuration)
        {
            _csvStreamReader = new CsvStreamReader(reader, configuration);
        }
        public TableDirect(StreamReader reader, CultureInfo cultureInfo)
        {
            _csvStreamReader = new CsvStreamReader(reader, new CsvConfiguration() { CultureInfo = cultureInfo });
        }
        public TableDirect(string table, DbConnection connection, CsvConfiguration configuration)
        {
            _csvSqlWriter = new CsvSqlWriter(table, connection, this, configuration);

        }
        public TableDirect(string table, DbConnection connection, string whereClasue, CsvConfiguration configuration)
        {
            _csvSqlReader = new CsvSqlReader(table, connection, whereClasue, configuration);
        }

        public async Task<bool> ReadAsync()
        {
            return await _csvStreamReader.ReadAsync();
        }
        public object GetValue(string name) => _csvStreamReader.GetValue(name);
        public T GetValue<T>(string name) => _csvStreamReader.GetValue<T>(name);
        public string GetValueAsString(string name) => _csvStreamReader.GetValueAsString(name);
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
                    result.Errors.Add($"Column at the index {index} is a type mismatch");
            }
            return result;
        }
        public async Task<int> CountAsync()
        {
            if (_count > 0) return _count;
            _csvStreamReader.Reset();
            while (await _csvStreamReader.ReadAsync())
                _count++;
            return _count;
        }
        public void Dispose()
        {
            _csvStreamReader.Dispose();
        }
        public async Task<TableDirect> ConvertToJsonAsync(string jsonColumnName)
        {
            var jsonConverter = new CsvJsonConverter(jsonColumnName, this, Header.ToSchema());
            return await jsonConverter.ConvertToJsonRowCollectionAsync();
        }
        public async Task<TableDirect> AddHashColumnColumnAsync(string hashColumnName)
        {
            if (Header != null) Header.Add(new CsvFieldHeader()
            {
                Ordinal = Header.Count,
                Schema = new CsvFieldSchema()
                {
                    Name = hashColumnName,
                    Type = typeof(string),
                }
            });
            while (await ReadAsync())
                Current.AddHashColumn();
            return this;
        }
        public async Task<TableDirect> EncryptDataAsync(string key, string[] columnExclusions = null)
        {
            var handler = new CsvCryptoHandler();
            Encrypted = true;
            while (await ReadAsync())
                Current.Encrypt(key, columnExclusions, handler, Header);
            return this;
        }
        public async Task<TableDirect> DecryptDataAsync(string key, string[] columnExclusions = null)
        {
            var handler = new CsvCryptoHandler();
            Encrypted = true;
            while (await ReadAsync())
                Current.Decrypt(key, columnExclusions, handler, Header);
            return this;
        }
        public async Task<int> WirteRowsToTableAsync() => await _csvSqlWriter.WirteRowsToTableAsync();
        public async Task<TableDirect> ReadRowsFromTableAsync() => await _csvSqlReader.ReadRowsFromTableAsync();
        public void AddHeader(int location, CsvFieldHeader header)
        {
            Header.Insert(location, header);
        }
    }
}
