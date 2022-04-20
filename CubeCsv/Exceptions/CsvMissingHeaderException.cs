using System;

namespace CubeCsv.Exceptions
{
    public class CsvMissingHeaderException : Exception
    {
        public CsvMissingHeaderException(string message) : base(message) { }
    }
}
