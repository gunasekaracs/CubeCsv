using System;

namespace CubeCsv.Exceptions
{
    public class CsvNullValueException : Exception
    {
        public CsvNullValueException(string message) : base(message) { }
    }
}
