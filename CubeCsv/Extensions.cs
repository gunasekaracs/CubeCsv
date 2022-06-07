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
            var indexes = new List<int>();

            for (int i = 0; i < value.Length; i++)
            {
                char charactor = value[i];
                if (charactor == '"')
                    quoted = true;
                if (quoted && charactor == '"' && (i + 1 == value.Length || value[i + 1] == delimiter))
                    quoted = false;
                if (quoted && charactor == delimiter)
                    indexes.Add(i);
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
