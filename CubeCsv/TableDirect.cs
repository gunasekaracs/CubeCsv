using CubeCsv.Exceptions;
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
        public TableDirect(string table, DbConnection connection, TableDirect tableDirect, CsvConfiguration configuration)
        {
            _csvSqlWriter = new CsvSqlWriter(table, connection, tableDirect, configuration);

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
        public void SetValue(string name, object value)
        {
            var header = Header["name"];
            if (header == null)
                throw new CsvMissingHeaderException("Header cannot be null");
            Current.SetValue(Header.GetOrdinal(name), header, value);
        }
        public void SetValue(int location, object value)
        {
            Current.SetValue(location, null, value);
        }
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
            if (_count > 0) return _count - _csvStreamReader.SkipRowCount;
            _csvStreamReader.Reset();
            while (await _csvStreamReader.ReadAsync())
                _count++;
            return _count - _csvStreamReader.SkipRowCount;
        }
        public void Dispose()
        {
            _csvStreamReader.Dispose();
        }
        public async Task<TableDirect> ConvertToJsonAsync(string jsonColumnName) =>
            await new CsvJsonConverter(jsonColumnName, this, _csvStreamReader.Configuration).ConvertToJsonRowCollectionAsync();

        public async Task<TableDirect> AddHashColumnColumnAsync(string hashColumnName) =>
            await new CsvHashGenerator(hashColumnName, this, _csvStreamReader.Configuration).GenerateHashAsync();

        public async Task<TableDirect> EncryptDataAsync(string key, string[] columnExclusions = null) =>
            await new CsvCryptoHandler().ConvertCrypto(true, this, key, Header, _csvStreamReader.Configuration, columnExclusions);

        public async Task<TableDirect> DecryptDataAsync(string key, string[] columnExclusions = null) =>
            await new CsvCryptoHandler().ConvertCrypto(false, this, key, Header, _csvStreamReader.Configuration, columnExclusions);

        public async Task<int> WirteToTableAsync() => await _csvSqlWriter.WirteToTableAsync();

        public async Task<TableDirect> ReadRowsFromTableAsync() => await _csvSqlReader.ReadRowsFromTableAsync();

        public async Task<TableDirect> AddColumnWithValue(int location, CsvFieldSchema schema, object value) =>
            await new CsvColumnManager().AddColumnWithValue(this, _csvStreamReader.Configuration, location, schema, value);

        public void AddHeader(int location, CsvFieldHeader header)
        {
            Header.Insert(location, header);
        }
    }
}
