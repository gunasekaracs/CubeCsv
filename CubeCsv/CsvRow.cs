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
        private enum ParsingMethod
        {
            Sql,
            Json
        }

        public void AddHashColumn()
        {
            var field = new CsvField();
            using (var computer = SHA256.Create())
            {
                string value = string.Join(string.Empty, this.Select(x => x.Value));
                string hash = string.Concat(computer.ComputeHash(Encoding.UTF8.GetBytes(value)).Select(item => item.ToString("x2")));
                field.Value = hash;
                Add(field);
            }
        }
        internal void Encrypt(string key, string[] columnExclusions, CsvCryptoHandler handler, CsvHeader header)
        {
            foreach (var field in this)
                if (columnExclusions == null || !columnExclusions.Any(x => x == header[IndexOf(field)].Schema.Name))
                    field.Encrypt(key, handler);
        }
        internal void Decrypt(string key, string[] columnExclusions, CsvCryptoHandler handler, CsvHeader header)
        {
            foreach (var field in this)
                if (columnExclusions == null || !columnExclusions.Any(x => x == header[IndexOf(field)].Schema.Name))
                    field.Decript(key, handler);
        }
        internal string ToSql(CsvHeader headers)
        {
            return ParseToString(ParsingMethod.Sql, headers);
        }
        internal string ToJson(CsvHeader headers)
        {
            return ParseToString(ParsingMethod.Json, headers);
        }
        public override string ToString()
        {
            return string.Join(",", this);
        }
        private string ParseToString(ParsingMethod method, CsvHeader headers)
        {
            var bracket = GetBracket(method);
            StringBuilder builder = new StringBuilder($"{bracket.Open}");
            if (Count != headers.Count)
                throw new CsvHeaderCountMismatchException($"Header count and field count does not match. Row has {Count} columns and header has {headers.Count}. Row = [{this}] and Header = [{headers}]");
            for (int i = 0; i < Count; i++)
            {
                CsvField field = this[i];
                CsvFieldHeader header = headers[i];
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
            builder.Append($"{bracket.Close}");
            return builder.ToString().Replace($",{bracket.Close}", $"{bracket.Close}");
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
