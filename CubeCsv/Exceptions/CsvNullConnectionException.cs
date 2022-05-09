using System;

namespace CubeCsv.Exceptions
{
    internal class CsvNullConnectionException : Exception
    {
        public CsvNullConnectionException(string message) : base(message) { }
    }
}
