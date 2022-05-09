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
        private Dictionary<Type, List<Type>> _allowedSwitches = new Dictionary<Type, List<Type>>()
        {
            { typeof(int), new List<Type>{ typeof(string), typeof(double),  typeof(float) } },
            { typeof(double), new List<Type>{ typeof(string) } },
            { typeof(float), new List<Type>{ typeof(string), typeof(double) } },
            { typeof(DateTime), new List<Type>{ typeof(string) } }
        };

        public CsvFieldHeader this[string name] => this.FirstOrDefault(x => x.Schema.Name == name);
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
            string delimiter = _configuration.Delimiter;
            if (_configuration.HasHeader)
            {
                string headerDelimiter = _configuration.Delimiter;
                if (_configuration.HeaderDoubleQuoted)
                    headerDelimiter = $"\"{ delimiter }\"";
                if (_reader.EndOfStream)
                    throw new CsvMissingHeaderException("There is no header row found");
                string headerLine = _reader.ReadLine();
                if (headerLine != null && string.IsNullOrWhiteSpace(headerLine))
                    throw new CsvMissingHeaderException("Invalid header line found");
                headerLine = Sanitize(headerLine, headerDelimiter);
                List<string> headers = new List<string>(headerLine.Split(delimiter.ToCharArray()));
                foreach (string header in headers)
                    Add(new CsvFieldHeader() { Schema = new CsvFieldSchema(header), Ordinal = headers.IndexOf(header) });
            }
        }

        public void Dispose()
        {
            _allowedSwitches = null;
        }
        public void ResolveSchema(string delimiter)
        {
            while (!_reader.EndOfStream)
            {
                string row = _reader.ReadLine();
                List<string> values = new List<string>(row.Split(delimiter.ToCharArray()));
                foreach (string field in values)
                {
                    if (string.IsNullOrWhiteSpace(field)) continue;
                    if (DateTime.TryParse(field, out DateTime _))
                        _ = SetType(values, field, typeof(DateTime));
                    else if (int.TryParse(field, out int _))
                        _ = SetType(values, field, typeof(int));
                    else if (float.TryParse(field, out float _))
                        _ = SetType(values, field, typeof(float));
                    else if (double.TryParse(field, out double _))
                        _ = SetType(values, field, typeof(double));
                    else
                    {
                        CsvFieldHeader header = SetType(values, field, typeof(string));
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
        private string Sanitize(string headerLine, string delimiter)
        {
            string marker = "##%##";
            headerLine = headerLine.Trim();
            headerLine = headerLine.Replace(delimiter, marker);
            headerLine = headerLine.Replace(_configuration.Delimiter, "##delimiter##");
            headerLine = headerLine.Replace(marker, _configuration.Delimiter);
            headerLine = headerLine.TrimStart('"');
            headerLine = headerLine.TrimEnd('"');
            return headerLine;
        }
        private CsvFieldHeader SetType(List<string> values, string field, Type type)
        {
            CsvFieldHeader header = null;
            int index = values.IndexOf(field);
            if (Count < index + 1)
                Add(header = new CsvFieldHeader() { Schema = new CsvFieldSchema(type) });
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
