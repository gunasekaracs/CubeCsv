using System;

namespace CubeCsv.Exceptions
{
    public class CsvInvalidSqlException : Exception
    {
        public CsvInvalidSqlException(string message) : base(message) { }
    }
}
