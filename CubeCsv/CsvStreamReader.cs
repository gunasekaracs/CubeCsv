using CubeCsv.Exceptions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CubeCsv
{
    sealed class CsvStreamReader : IDisposable
    {
        private CsvRow _row;
        private int _location = 0;
        private StreamReader _reader;
        private CsvConfiguration _configuration = new CsvConfiguration()    ;
        private CsvHeader _header;
        private readonly int _skipRowCount = 0;
        private readonly bool _breakOnError = true;
        private readonly List<CsvRowReadError> _errors = new List<CsvRowReadError>();
        private int _count = 0;
        private int _errorCount = 0;
        private readonly bool _includeDataInLogs = true;

        public CsvHeader Header { get { return _header; } }
        public CsvConfiguration Configuration
        {
            get { return _configuration; }
            set { _configuration = value; }
        }
        public CsvRow Current { get { return _row; } }
        public int Location => _location - 1;
        public int SkipRowCount => _skipRowCount;
        public List<CsvRowReadError> Errors => _errors;
        public int ErrorCount => _errorCount;

        public CsvStreamReader(StreamReader reader, CsvConfiguration configuration)
        {
            _configuration = configuration;
            if (_configuration.RemoveLineBreaks)
                reader = RemoveLineBreaks(reader);
            _includeDataInLogs = _configuration.IncludeDataInLogs;
            _reader = reader;
            _header = new CsvHeader(_configuration, _reader);
            _header.ResolveSchema(_configuration.Delimiter);
            _skipRowCount = _configuration.SkipRowCount;
            _breakOnError = configuration.BreakOnError;
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
                while (!ReadRow(row))
                {
                    row = await _reader.ReadLineAsync();
                    _errorCount++;
                    if (_reader.EndOfStream)
                    {
                        Reset();
                        return false;
                    }
                }
            _location++;
            return true;
        }
        public object GetValue(string name)
        {

            return _row[Header.GetOrdinal(name)]?.Value;
        }
        public object GetValue(int column)
        {
            return _row[column]?.Value;
        }
        public T GetValue<T>(string name)
        {
            var field = _row[Header.GetOrdinal(name)];
            return GetValue<T>(field);
        }
        public T GetValue<T>(int column)
        {
            return GetValue<T>(_row[column]);
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
            var header = Header[name] ?? throw new CsvMissingHeaderException("Header cannot be null");
            Current.SetValue(Header.GetOrdinal(name), header, value);
        }
        private bool ReadRow(string row)
        {
            _row = new CsvRow();
            char delimiter = _configuration.Delimiter;
            if (_configuration.RemoveLineBreaks)
                row = row.Replace(Environment.NewLine, string.Empty);
            row = row.Trim();
            if (_configuration.RowCleaner != null)
                row = _configuration.RowCleaner.Clean(row);
            row = row.ReplaceRequiredCommas(delimiter);
            var values = new List<string>(row.Split(delimiter)).Select(x => x.RestoreCommas(delimiter)).ToList();
            if (values.Count != Header.Count)
            {
                AddError($"Header count and field count does not match. Row has {values.Count} columns and header has {Header.Count}. {(_includeDataInLogs ? $"Row = [{string.Join(",", values)}] and" : string.Empty)} Header = [{Header}]", CsvRowReadError.Type.Header);
                return false;
            }
            int index = 0;
            bool success = true;
            foreach (string value in values)
            {
                var header = Header[index];
                var fieldResult = ResolveValue(value, header.Schema.Type, header.Ordinal);
                if (fieldResult.IsSuccess)
                {
                    var result = ValidateField(fieldResult.Value, header, _location);
                    if (result.IsValid)
                        _row.Add(new CsvField() { Value = fieldResult.Value, Ordinal = index++ });
                    else
                    {
                        success = false;
                        AddError($"Error at the row {_location + _errorCount}, {result.Error}", CsvRowReadError.Type.Field);
                    }
                }
                else
                {
                    success = false;
                    AddError($"Error at the row {_location + _errorCount}, {fieldResult.Error}", CsvRowReadError.Type.Field);
                }
            }
            return success;
        }
        private (bool IsSuccess, object Value, string Error) ResolveValue(string value, Type type, int column)
        {
            if (_configuration.CellCleaner != null)
                value = _configuration.CellCleaner.Clean(value);
            if (string.IsNullOrWhiteSpace(value))
                return (true, string.Empty, string.Empty);
            if (type == typeof(DateTime))
            {
                if (DateTime.TryParse(value, out DateTime result))
                    return (true, result, string.Empty);
                else
                    return (false, string.Empty, $"column {column}, {LogValue(value)} cannot be converted to Date Time");
            }
            if (type == typeof(double))
            {
                if (double.TryParse(value, out double result))
                    return (true, result, string.Empty);
                else
                    return (false, string.Empty, $"column {column}, {LogValue(value)} cannot be converted to double");
            }
            if (type == typeof(float))
            {
                if (float.TryParse(value, out float result))
                    return (true, result, string.Empty);
                else
                    return (false, string.Empty, $"column {column}, {LogValue(value)} cannot be converted to float");
            }
            if (type == typeof(long))
            {
                if (long.TryParse(value, out long result))
                    return (true, result, string.Empty);
                else
                    return (false, string.Empty, $"column {column}, {LogValue(value)} cannot be converted to long");
            }
            if (type == typeof(int))
            {
                if (int.TryParse(value, out int result))
                    return (true, result, string.Empty);
                else
                    return (false, string.Empty, $"column {column}, {LogValue(value)} cannot be converted to int");
            }
            if (value.StartsWith("\"")) value = value.Substring(1, value.Length - 1);
            if (value.EndsWith("\"")) value = value.Substring(0, value.Length - 1);
            return (true, value.Trim(), string.Empty);
        }
        public (bool IsValid, string Error) ValidateField(object value, CsvFieldHeader header, int index)
        {
            if (value == null || string.IsNullOrEmpty(value.ToString())) return (true, string.Empty);
            if (header == null)
                throw new CsvMissingHeaderException("Validate a field you should provide a column header");
            if (header.Schema == null)
                throw new CsvNullSchemaException("Validate a field you should provide valid header schema");
            if (header.Schema.Type != value.GetType())
                return (false, $"schema type [{header.Schema.Type}] and value type [{value.GetType()}] does not match at the column {header.Ordinal}");
            else if (header.Schema.Type == typeof(string) && header.Schema.Length > 0 && value.ToString().Length > header.Schema.Length)
                return (false, $"Provided {LogValue(value)} too large to fit in this column {header.Ordinal}. Schema length [{header.Schema.Length}] is but length of the value is [{value.ToString().Length}]");
            else
            {
                var validator = header.Schema.Validator;
                if (validator != null)
                {
                    if (validator.Type == CsvFieldValidator.ValidatorType.Regex)
                    {
                        if (string.IsNullOrEmpty(validator.RegularExpression))
                            throw new CsvNullValueException($"column {header.Ordinal}, You have to setup regular expression in the schema, when setting up the column {index} to validated by regular expression");
                        if (!Regex.IsMatch(value.ToString(), validator.RegularExpression))
                            return (false, $"column {header.Ordinal}, {LogValue(value)} is not in the correct format {validator.Description}".Trim());
                    }
                }
            }
            return (true, string.Empty);
        }
        private T GetValue<T>(CsvField field)
        {
            if (field.Value is T value)
                return value;
            throw
                new CsvInvalidCastException($"Unable to convert type {field.Value.GetType()} into a type of {typeof(T)}");
        }
        private StreamReader RemoveLineBreaks(StreamReader reader)
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            StreamReader result = new StreamReader(stream);
            bool quoted = false;
            bool spaced = false;
            reader.BaseStream.Seek(0, SeekOrigin.Begin);
            while (!reader.EndOfStream)
            {
                char charactor = (char)reader.Read();
                if (charactor == '"')
                {
                    quoted = !quoted;
                    spaced = false;
                }
                if (quoted && (charactor == '\n' || charactor == '\r')) continue;
                if (quoted && charactor == ' ')
                {
                    if (spaced) continue;
                    spaced = true;
                }
                writer.Write(charactor);
            }
            writer.Flush();
            result.BaseStream.Seek(0, SeekOrigin.Begin);
            return new StreamReader(stream);
        }
        private void AddError(string message, CsvRowReadError.Type type)
        {
            if (_breakOnError)
                throw CreateException(type, message);
            else
                _errors.Add(new CsvRowReadError() { Error = message, RowNumber = _location });
        }
        private Exception CreateException(CsvRowReadError.Type type, string message)
        {
            switch (type)
            {
                case CsvRowReadError.Type.Header:
                    return new CsvInvalidHeaderException(message);
                case CsvRowReadError.Type.Field:
                    return new CsvInvalidFieldException(message);
                default:
                    throw new NotImplementedException("Type of this exception is not implemeted");
            }
        }
        private string LogValue(object value)
        {
            return $"value{(_includeDataInLogs ? $" [{value}]" : string.Empty)}";
        }
    }
}
