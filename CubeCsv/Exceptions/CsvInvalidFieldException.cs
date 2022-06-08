using System;

namespace CubeCsv.Exceptions
{
    public class CsvInvalidFieldException: Exception
    {
        public CsvInvalidFieldException(string message) : base(message) { }
    }
}
