using CubeCsv.Exceptions;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;

namespace CubeCsv
{
    public class CsvFile : IDisposable
    {
        private CsvStreamReader _csvStreamReader;
        private CsvSqlWriter _csvSqlWriter;
        private CsvSqlReader _csvSqlReader;
        
        public bool Encrypted { get; private set; }

        public CsvRow Current => _csvStreamReader.Current;
        public CsvHeader Header => _csvStreamReader.Header;
        public int Location => _csvStreamReader.Location;
        public bool IsEmpty => CountAsync().Result == 0;

        public CsvFile(StreamReader reader, CsvConfiguration configuration)
        {
            _csvStreamReader = new CsvStreamReader(reader, configuration);
        }
        public CsvFile(StreamReader reader, CultureInfo cultureInfo)
        {
            _csvStreamReader = new CsvStreamReader(reader, new CsvConfiguration() { CultureInfo = cultureInfo });
        }
        public CsvFile(string table, DbConnection connection, CsvFile tableDirect, CsvConfiguration configuration)
        {
            _csvSqlWriter = new CsvSqlWriter(table, connection, tableDirect, configuration);

        }
        public CsvFile(string table, DbConnection connection, string whereClasue, string orderBy, CsvConfiguration configuration)
        {
            _csvSqlReader = new CsvSqlReader(table, connection, whereClasue, orderBy, configuration);
        }

        public async Task<bool> ReadAsync()
        {
            return await _csvStreamReader.ReadAsync();
        }
        public object GetValue(string name) => _csvStreamReader.GetValue(name);
        public T GetValue<T>(string name) => _csvStreamReader.GetValue<T>(name);
        public string GetValueAsString(string name) => _csvStreamReader.GetValueAsString(name);
        public void SetValue(string name, object value) => _csvStreamReader.SetValue(name, value);
        public void SetValue(int location, object value) => Current.SetValue(location, null, value);
        public CsvValidationResult ValidateSchema(CsvSchema schema) => new CsvValidator().ValidateSchema(this, schema.ToArray());
        public CsvValidationResult ValidateSchema(params CsvFieldSchema[] fields) => new CsvValidator().ValidateSchema(this, fields);
        public async Task<CsvValidationResult> Validate() => await new CsvValidator().ValidateAsync(this);
        public async Task<int> CountAsync() => await _csvStreamReader.CountAsync();   
        public async Task<CsvFile> ConvertToJsonAsync(string jsonColumnName) =>
            await new CsvJsonConverter(jsonColumnName, this, _csvStreamReader.Configuration).ConvertToJsonRowCollectionAsync();
        public async Task<CsvFile> AddHashColumnColumnAsync(string hashColumnName) =>
            await new CsvHashGenerator(hashColumnName, this, _csvStreamReader.Configuration).GenerateHashAsync();
        public async Task<CsvFile> EncryptDataAsync(string key, string[] columnExclusions = null) =>
            await new CsvCryptoHandler().ConvertCrypto(true, this, key, Header, _csvStreamReader.Configuration, columnExclusions);
        public async Task<CsvFile> DecryptDataAsync(string key, string[] columnExclusions = null) =>
            await new CsvCryptoHandler().ConvertCrypto(false, this, key, Header, _csvStreamReader.Configuration, columnExclusions);
        public async Task<int> WirteToTableAsync() => await _csvSqlWriter.WirteToTableAsync();
        public async Task<CsvFile> ReadRowsFromTableAsync() => await _csvSqlReader.ReadRowsFromTableAsync();
        public async Task<CsvFile> AddColumnWithValue(int location, CsvFieldSchema schema, object value) =>
            await new CsvColumnManager().AddColumnWithValue(this, _csvStreamReader.Configuration, location, schema, value);
        public void AddHeader(int location, CsvFieldHeader header) => Header.Insert(location, header);
        public void Dispose() => _csvStreamReader.Dispose();
    }
}
