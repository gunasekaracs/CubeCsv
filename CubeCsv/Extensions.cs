using System.Collections.Generic;
using System.Text;

namespace CubeCsv
{
    public static class Extensions
    {
        private static char comma = '∙';
        public static List<int> GetQuotedCommas(this string value, char delimiter)
        {
            bool quoted = false;
            bool json = false;
            bool jsonString = false;
            var indexes = new List<int>();

            for (int i = 0; i < value.Length; i++)
            {
                char charactor = value[i];
                if (charactor == '"' && !quoted)
                {
                    quoted = true;
                    jsonString = json = false;
                }
                if (delimiter == ',' && quoted && charactor == ':' && value[i - 1] == '"')
                {
                    json = true;
                    if (i + 1 < value.Length && value[i + 1] == '"')
                        jsonString = true;
                }
                if (delimiter == ',' && quoted && json && value[i] == '}')
                    json = jsonString = false;
                if (quoted && charactor == '"' && (i + 1 == value.Length || value[i + 1] == delimiter) && !json && (!jsonString || value[i - 1] == '"'))
                {
                    quoted = false;
                    jsonString = json = false;
                }
                if (quoted && charactor == delimiter)
                {
                    jsonString = json = false;
                    indexes.Add(i);
                }
            }

            return indexes;
        }
        public static string ReplaceRequiredCommas(this string value, char delimiter)
        {
            List<int> indexes = GetQuotedCommas(value, delimiter);

            var builder = new StringBuilder(value);
            foreach (int i in indexes)
                builder[i] = comma;
            value = builder.ToString();
            value = value.Replace($"{comma} ", comma.ToString());

            return value;
        }
        public static string RestoreCommas(this string value, char delimiter)
        {
            return value.Replace(comma.ToString(), delimiter.ToString());
        }
    }
}
