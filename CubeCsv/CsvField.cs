using System;
using System.Text;

namespace CubeCsv
{
    public class CsvField : CsvOrdinal
    {
        public object Value { get; set; }

        public override string ToString()
        {
            return (Value ?? string.Empty).ToString();
        }
        public string ToSql(CsvFieldHeader header)
        {
            StringBuilder builder = new StringBuilder();
            Type type = header.Schema.Type;
            if (type == typeof(int) || type == typeof(float) || type == typeof(double))
                builder.Append(Value);
            else
                builder.Append($"'{ Value }'");
            return builder.ToString();
        }
        public string ToJson(CsvFieldHeader header)
        {
            StringBuilder builder = new StringBuilder();
            Type type = header.Schema.Type;
            builder.Append($"\"{ header.Schema.Name }\":");
            if (type == typeof(int) || type == typeof(float) || type == typeof(double))
                builder.Append($"{ Value }");
            else
                builder.Append($"\"{ Value }\"");
            return builder.ToString();
        }
        public void Encrypt(string key)
        {
            if (Value == null)
                throw new ArgumentNullException("Value of the csv field is null, so unable to encrypt, please review");
            Value = new CsvCryptoHandler().Encrypt(Value.ToString(), key);
        }
        public void Decript(string key)
        {
            if (Value == null)
                throw new ArgumentNullException("Value of the csv field is null, so unable to encrypt, please review");
            Value = new CsvCryptoHandler().Decrypt(Value.ToString(), key);
        }
    }
}
