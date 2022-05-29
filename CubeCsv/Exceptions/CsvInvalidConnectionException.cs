using System;

namespace CubeCsv.Exceptions
{
    public class CsvInvalidConnectionException : Exception
    {
        public CsvInvalidConnectionException(string message) : base(message) { }
    }
}
