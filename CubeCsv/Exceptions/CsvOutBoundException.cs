using System;

namespace CubeCsv.Exceptions
{
    public class CsvOutBoundException:Exception
    {
        public CsvOutBoundException(string message) : base(message) { }
    }
}
