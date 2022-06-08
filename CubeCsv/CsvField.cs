using System;
using System.Text;

namespace CubeCsv
{
    public class CsvField : CsvOrdinal
    {
        public object Value { get; set; }

        public string ToString(char delimiter)
        {
            string value = (Value ?? string.Empty).ToString();
            if ((value.Contains(delimiter.ToString()) || value.Contains("\"")) && !value.StartsWith("\"") && !value.EndsWith("\""))
                return $"\"{value}\"";
            return value;
        }
        public string ToSql(CsvFieldHeader header)
        {
            StringBuilder builder = new StringBuilder();
            Type type = header.Schema.Type;
            if (type == typeof(int) || type == typeof(float) || type == typeof(double))
                builder.Append(Value);
            else
                builder.Append($"'{Value}'");
            return builder.ToString();
        }
        public string ToJson(CsvFieldHeader header)
        {
            StringBuilder builder = new StringBuilder();
            Type type = header.Schema.Type;
            builder.Append($"\"{header.Schema.Name}\":");
            if (type == typeof(int) || type == typeof(float) || type == typeof(double))
                builder.Append($"{Value}");
            else
                builder.Append($"\"{Value}\"");
            return builder.ToString();
        }
        public void Encrypt(string key, CsvCryptoHandler handler)
        {
            if (Value == null)
                throw new ArgumentNullException("Value of the csv field is null, so unable to encrypt, please review");
            Value = handler.EncryptValue(Value.ToString(), key);
        }
        public void Decript(string key, CsvCryptoHandler handler)
        {
            if (Value == null)
                throw new ArgumentNullException("Value of the csv field is null, so unable to encrypt, please review");
            Value = handler.DecryptValue(Value.ToString(), key);
        }
    }
}
