using CubeCsv.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CubeCsv
{
    public sealed class CsvHeader : List<CsvFieldHeader>, IDisposable
    {
        private CsvConfiguration _configuration = new CsvConfiguration();
        private StreamReader _reader;
        private bool _requireLenghDetection;
        private Dictionary<Type, List<Type>> _allowedSwitches = new Dictionary<Type, List<Type>>()
        {
            { typeof(int), new List<Type>{ typeof(string), typeof(double),  typeof(float) } },
            { typeof(double), new List<Type>{ typeof(string) } },
            { typeof(float), new List<Type>{ typeof(string), typeof(double) } },
            { typeof(DateTime), new List<Type>{ typeof(string) } }
        };

        public CsvFieldHeader this[string name] => this.SingleOrDefault(x => x.Schema.Name == name);
        public CsvConfiguration Configuration
        {
            get { return _configuration; }
            set { _configuration = value; }
        }

        public CsvHeader() { }
        public CsvHeader(CsvConfiguration configuration, StreamReader reader)
        {
            _configuration = configuration;
            _reader = reader;
            if (configuration.Schema != null)
            {
                foreach (var item in configuration.Schema)
                    Add(new CsvFieldHeader()
                    {
                        Schema = new CsvFieldSchema(item.Name, item.Type, item.Length, item.Validator),
                        Ordinal = configuration.Schema.IndexOf(item)
                    });
            }
            else
            {
                _requireLenghDetection = true;
                char delimiter = _configuration.Delimiter;
                if (_configuration.HasHeader)
                {
                    string headerDelimiter = _configuration.Delimiter.ToString();
                    if (_configuration.HeaderDoubleQuoted)
                        headerDelimiter = $"\"{delimiter}\"";
                    if (_reader.EndOfStream)
                        throw new CsvMissingHeaderException("There is no header row found");
                    string headerLine = _reader.ReadLine();
                    if (headerLine != null && string.IsNullOrWhiteSpace(headerLine))
                        throw new CsvMissingHeaderException("Invalid header line found");
                    headerLine = Sanitize(headerLine, headerDelimiter);
                    List<string> headers = new List<string>(headerLine.Split(delimiter));
                    foreach (string header in headers)
                        Add(new CsvFieldHeader() { Schema = new CsvFieldSchema(header), Ordinal = headers.IndexOf(header) });
                }
            }
        }

        public void Dispose()
        {
            _allowedSwitches = null;
        }
        public void ResolveSchema(char delimiter)
        {
            if (!_requireLenghDetection) return;
            while (!_reader.EndOfStream)
            {
                string row = _reader.ReadLine();
                row = row.ReplaceRequiredCommas(delimiter);
                List<string> values = new List<string>(row.Split(delimiter));

                for (int i = 0; i < values.Count; i++)
                {
                    var field = values[i];
                    if (!string.IsNullOrEmpty(field)) 
                        field = field.RestoreCommas(delimiter);
                    if (string.IsNullOrWhiteSpace(field)) continue;
                    if (DateTime.TryParse(field, out DateTime _))
                        _ = SetType(values, field, typeof(DateTime), i);
                    else if (int.TryParse(field, out int _))
                        _ = SetType(values, field, typeof(int), i);
                    else if (float.TryParse(field, out float _))
                        _ = SetType(values, field, typeof(float), i);
                    else if (double.TryParse(field, out double _))
                        _ = SetType(values, field, typeof(double), i);
                    else
                    {
                        CsvFieldHeader header = SetType(values, field, typeof(string), i);
                        int length = (field ?? string.Empty).Trim().Length;
                        if (!string.IsNullOrWhiteSpace(field) && length > header.Schema.Length)
                            header.Schema.Length = length;
                    }
                }
            }
            foreach (CsvFieldHeader header in this)
                if (header.Schema.Type == null)
                    header.Schema.Type = typeof(string);
            _reader.BaseStream.Position = 0;
            if (_configuration.HasHeader)
                _reader.ReadLine();
        }
        public CsvSchema ToSchema()
        {
            CsvSchema schema = new CsvSchema();
            foreach (CsvFieldHeader header in this)
                schema.Add(header.Schema);
            return schema;
        }
        public override string ToString()
        {
            return string.Join(",", this);
        }
        public int GetOrdinal(string name)
        {
            var header = this[name];
            if (header == null)
                throw new CsvMissingHeaderException($"Unable to find the [{name}] in the headers collection of [{this}]");
            return header.Ordinal;
        }

        private string Sanitize(string headerLine, string delimiter)
        {
            string marker = "##%##";
            headerLine = headerLine.Trim();
            headerLine = headerLine.Replace(delimiter, marker);
            headerLine = headerLine.Replace(_configuration.Delimiter.ToString(), "##delimiter##");
            headerLine = headerLine.Replace(marker, _configuration.Delimiter.ToString());
            headerLine = headerLine.TrimStart('"');
            headerLine = headerLine.TrimEnd('"');
            return headerLine;
        }
        private CsvFieldHeader SetType(List<string> values, string field, Type type, int index)
        {
            CsvFieldHeader header = null;
            if (Count < index + 1)
                Add(header = new CsvFieldHeader()
                {
                    Ordinal = index,
                    Schema = new CsvFieldSchema(type)
                });
            else
            {
                header = this[index];
                Type existing = header.Schema.Type;
                if (existing == null)
                {
                    header.Schema.Type = type;
                    return header;
                }
                if (existing == type || existing == typeof(string)) return header;
                if (_allowedSwitches[existing].Any(x => x == type))
                    header.Schema.Type = type;
            }
            return header;
        }
    }
}
