using CubeCsv.Exceptions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;

namespace CubeCsv
{
    public sealed class CsvReader : CsvBase, IDisposable
    {
        private CsvRow _row;
        private int _index = 0;

        public CsvRow Current { get { return _row; } }

        public CsvReader(StreamReader reader, CsvConfiguration configuration) : base(configuration, reader) { }
        public CsvReader(StreamReader reader, CultureInfo cultureInfo) : base(new CsvConfiguration() { CultureInfo = cultureInfo }, reader) { }
        public async Task<bool> ReadAsync()
        {
            if (_reader.EndOfStream) return false;
            if (_index < _skipRowCount)
                await _reader.ReadLineAsync();
            else
            {
                string row = await _reader.ReadLineAsync();
                if (!string.IsNullOrWhiteSpace(row))
                    ReadRow(row);
            }
            _index++;
            return true;
        }
        public object GetValue(string name)
        {
            return _row[name].Value;
        }
        public T GetValue<T>(string name)
        {
            var field = _row[name];
            if (field.Value is T value)
                return value;
            throw
                new CsvInvalidCastException($"Unable to convert type { field.Value.GetType() } into a type of { typeof(T) }");
        }
        public string GetValueAsString(string name)
        {
            return _row[name].Value.ToString();
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
            _row = new CsvRow(_header, _configuration.HasHeader);
            char delimiter = char.Parse(_configuration.Delimiter);
            if (_configuration.RemoveLineBreaks)
                row = row.Replace(Environment.NewLine, string.Empty);
            if (_configuration.RowCleaner != null)
                row = _configuration.RowCleaner.Clean(row);
            List<string> values = new List<string>(row.Split(delimiter));
            if (values.Count != Header.Count)
                throw new CsvHeaderCountMismatchException($"Header count and field count does not match. Row has { values.Count } columns and header has { Header.Count }. Row = [{ string.Join(',', values) }] and Header = [{ Header }]");
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
            if (type == typeof(int)) return int.Parse(value);
            return value.Trim();
        }
    }
}
