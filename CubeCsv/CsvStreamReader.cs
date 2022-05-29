using CubeCsv.Exceptions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;

namespace CubeCsv
{
    public sealed class CsvStreamReader : IDisposable
    {
        private CsvRow _row;
        private int _location = 0;
        private StreamReader _reader;
        private CsvConfiguration _configuration = new CsvConfiguration();
        private CsvHeader _header;
        private int _skipRowCount = 1;        

        public CsvHeader Header { get { return _header; } }
        public CsvConfiguration Configuration
        {
            get { return _configuration; }
            set { _configuration = value; }
        }
        public CsvRow Current { get { return _row; } }
        public int Location => _location - 1;

        public CsvStreamReader(StreamReader reader, CsvConfiguration configuration)
        {
            _configuration = configuration;
            _reader = reader;
            _header = new CsvHeader(_configuration, _reader);
            _header.ResolveSchema(_configuration.Delimiter);
            _skipRowCount = _configuration.SkipRowCount;

        }
        public CsvStreamReader(StreamReader reader, CultureInfo cultureInfo) : this(reader, new CsvConfiguration() { CultureInfo = cultureInfo }) { }

        public async Task<bool> ReadAsync()
        {
            if (_reader.EndOfStream)
            {
                Reset();
                return false;
            }
            if (_location < _skipRowCount)
                await _reader.ReadLineAsync();
            else
            {
                string row = await _reader.ReadLineAsync();
                if (!string.IsNullOrWhiteSpace(row))
                    ReadRow(row);
            }
            _location++;
            return true;
        }
        public object GetValue(string name)
        {

            return _row[Header.GetOrdinal(name)]?.Value;
        }
        public T GetValue<T>(string name)
        {
            var field = _row[Header.GetOrdinal(name)];
            if (field.Value is T value)
                return value;
            throw
                new CsvInvalidCastException($"Unable to convert type {field.Value.GetType()} into a type of {typeof(T)}");
        }
        public string GetValueAsString(string name)
        {
            return _row[Header.GetOrdinal(name)]?.Value?.ToString();
        }
        public void Reset()
        {
            _location = 0;
            _reader.BaseStream.Seek(0, SeekOrigin.Begin);
            if (_configuration.HasHeader)
                _reader.ReadLine();
        }
        public void Dispose()
        {
            _header.Dispose();
            _reader = null;
            _configuration = null;
            _header = null;
        }
        private void ReadRow(string row)
        {
            _row = new CsvRow();
            char delimiter = char.Parse(_configuration.Delimiter);
            if (_configuration.RemoveLineBreaks)
                row = row.Replace(Environment.NewLine, string.Empty);
            if (_configuration.RowCleaner != null)
                row = _configuration.RowCleaner.Clean(row);
            List<string> values = new List<string>(row.Split(delimiter));
            if (values.Count != Header.Count)
                throw new CsvHeaderCountMismatchException($"Header count and field count does not match. Row has {values.Count} columns and header has {Header.Count}. Row = [{string.Join(",", values)}] and Header = [{Header}]");
            int index = 0;
            foreach (string value in values)
                _row.Add(new CsvField() { Value = ResolveValue(value, Header[index].Schema.Type), Ordinal = index++ });
        }
        private object ResolveValue(string value, Type type)
        {
            if (_configuration.CellCleaner != null)
                value = _configuration.CellCleaner.Clean(value);
            if (string.IsNullOrWhiteSpace(value))
                return string.Empty;
            if (type == typeof(DateTime)) return DateTime.Parse(value);
            if (type == typeof(double)) return double.Parse(value);
            if (type == typeof(float)) return float.Parse(value);
            if (type == typeof(long)) return long.Parse(value);
            if (type == typeof(int)) return int.Parse(value);
            return value.Trim();
        }
    }
}
