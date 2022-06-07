using CubeCsv.Exceptions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CubeCsv
{
    sealed class CsvStreamReader : IDisposable
    {
        private CsvRow _row;
        private int _location = 0;
        private StreamReader _reader;
        private CsvConfiguration _configuration = new CsvConfiguration();
        private CsvHeader _header;
        private int _skipRowCount = 0;
        private int _count;
        private char comma = '∙';

        public CsvHeader Header { get { return _header; } }
        public CsvConfiguration Configuration
        {
            get { return _configuration; }
            set { _configuration = value; }
        }
        public CsvRow Current { get { return _row; } }
        public int Location => _location - 1;
        public int SkipRowCount => _skipRowCount;   

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

            while (_location < _skipRowCount)
            {
                await _reader.ReadLineAsync();
                _location++;
                if (_reader.EndOfStream)
                {
                    Reset();
                    return false;
                }
            }

            string row = await _reader.ReadLineAsync();
            if (!string.IsNullOrWhiteSpace(row))
                ReadRow(row);

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
        public async Task<int> CountAsync()
        {
            if (_count > 0) return _count;
            Reset();
            while (await ReadAsync())
                _count++;
            return _count;
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
        public void SetValue(string name, object value)
        {
            var header = Header["name"];
            if (header == null)
                throw new CsvMissingHeaderException("Header cannot be null");
            Current.SetValue(Header.GetOrdinal(name), header, value);
        }
        private void ReadRow(string row)
        {
            _row = new CsvRow();
            char delimiter = char.Parse(_configuration.Delimiter);
            if (_configuration.RemoveLineBreaks)
                row = row.Replace(Environment.NewLine, string.Empty);
            if (_configuration.RowCleaner != null)
                row = _configuration.RowCleaner.Clean(row);
            row = ReplaceRequiredCommas(row, delimiter);
            var values = new List<string>(row.Split(delimiter)).Select(x => RestoreCommas(x, delimiter)).ToList();
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
        private List<int> GetQuotedCommas(string value, char delimiter)
        {
            bool quoted = false;
            var indexes = new List<int>();

            for (int i = 0; i < value.Length; i++)
            {
                char charactor = value[i];
                if (charactor == '"') quoted = !quoted;
                if (quoted && charactor == delimiter) indexes.Add(i);
            }

            return indexes;
        }
        private string ReplaceRequiredCommas(string value, char delimiter)
        {
            List<int> indexes = GetQuotedCommas(value, delimiter);

            var builder = new StringBuilder(value);
            foreach (int i in indexes)
                builder[i] = comma;
            value = builder.ToString();
            value = value.Replace($"{comma} ", comma.ToString());

            return value;
        }
        private string RestoreCommas(string value, char delimiter)
        {
            return value.Replace(comma.ToString(), delimiter.ToString());
        }
    }
}
