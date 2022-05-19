using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using CubeCsv.Exceptions;

namespace CubeCsv
{
    public class CsvRow : List<CsvField>
    {
        private CsvHeader _header;
        private bool _hasHeader;
        private enum ParsingMethod
        {
            Sql,
            Json
        }

        public CsvHeader Header
        {
            get { return _header; }
            set { _header = value; }
        }
        public bool HasHeader => _hasHeader;
        public CsvRow(CsvHeader header, bool hasHeader)
        {
            _header = header;
            _hasHeader = hasHeader;
        }
        public CsvField this[string name]
        {
            get
            {
                CsvFieldHeader header = _header[name];
                return this.FirstOrDefault(x => x.Ordinal == header.Ordinal);
            }
        }
        public void AddHashColumn(string hashColumnName)
        {
            var field = new CsvField();
            if (Header != null) Header.Add(new CsvFieldHeader()
            {
                Ordinal =
                Header.Count,
                Schema = new CsvFieldSchema()
                {
                    Name = hashColumnName,
                    Type = typeof(string),
                }
            });
            using (var computer = SHA256.Create())
            {
                string value = string.Join(string.Empty, this.Select(x => x.Value));
                string hash = string.Concat(computer.ComputeHash(Encoding.UTF8.GetBytes(value)).Select(item => item.ToString("x2")));
                field.Value = hash;
                Add(field);
            }
        }
        public void Encrypt(string key, string[] columnExclusions)
        {
            foreach (var field in this)
                if (!columnExclusions.Any(x => x == Header[this.IndexOf(field)].Schema.Name))
                    field.Encrypt(key);
        }
        public void Decrypt(string key, string[] columnExclusions)
        {
            foreach (var field in this)
                if (!columnExclusions.Any(x => x == Header[this.IndexOf(field)].Schema.Name))
                    field.Decript(key);
        }
        public string ToSql()
        {
            return ParseToString(ParsingMethod.Sql);
        }
        public string ToJson()
        {
            return ParseToString(ParsingMethod.Json);
        }
        public override string ToString()
        {
            return String.Join(",", this);
        }
        private string ParseToString(ParsingMethod method)
        {
            var bracket = GetBracket(method);
            StringBuilder builder = new StringBuilder($"{ bracket.Open }");
            if (Count != _header.Count)
                throw new CsvHeaderCountMismatchException($"Header count and field count does not match. Row has { Count } columns and header has { _header.Count }. Row = [{ this }] and Header = [{ _header }]");
            for (int i = 0; i < Count; i++)
            {
                CsvField field = this[i];
                CsvFieldHeader header = _header[i];
                switch (method)
                {
                    case ParsingMethod.Json:
                        builder.Append(field.ToJson(header));
                        break;
                    case ParsingMethod.Sql:
                        builder.Append(field.ToSql(header));
                        break;
                    default:
                        throw new NotImplementedException();
                }
                builder.Append(",");
            }
            builder.Append($"{ bracket.Close }");
            return builder.ToString().Replace($",{ bracket.Close }", $"{ bracket.Close }");
        }
        private (string Open, string Close) GetBracket(ParsingMethod method)
        {
            switch (method)
            {
                case ParsingMethod.Json:
                    return ("{", "}");
                case ParsingMethod.Sql:
                    return ("(", ")");
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
