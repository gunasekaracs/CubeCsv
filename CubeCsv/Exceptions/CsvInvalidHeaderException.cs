using System;

namespace CubeCsv.Exceptions
{
    public class CsvInvalidHeaderException : Exception
    {
        public CsvInvalidHeaderException(string message) : base(message) { }
    }
}
