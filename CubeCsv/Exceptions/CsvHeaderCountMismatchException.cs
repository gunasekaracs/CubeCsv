using System;

namespace CubeCsv.Exceptions
{
    internal class CsvHeaderCountMismatchException : Exception
    {
        public CsvHeaderCountMismatchException(string message) : base(message) { }
    }
}
