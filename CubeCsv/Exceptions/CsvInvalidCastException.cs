using System;

namespace CubeCsv.Exceptions
{
    public class CsvInvalidCastException : Exception
    {
        public CsvInvalidCastException(string message) : base(message) { }
    }
}
