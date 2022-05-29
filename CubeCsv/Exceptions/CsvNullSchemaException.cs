using System;

namespace CubeCsv.Exceptions
{
    public class CsvNullSchemaException : Exception
    {
        public CsvNullSchemaException(string message) : base(message) { }
    }
}
