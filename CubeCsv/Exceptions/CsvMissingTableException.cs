using System;

namespace CubeCsv.Exceptions
{
    internal class CsvMissingTableException : Exception
    {
        public CsvMissingTableException(string message) : base(message) { }
    }
}
