using System.Collections.Generic;
using System.Linq;

namespace CubeCsv
{
    public class CsvValidationResult
    {
        private bool _success = true;

        public bool Success
        {
            get { return _success && !HasErrors; }
            set { _success = value; }
        }
        public bool HasErrors => Errors.Any();
        public List<CsvRowReadError> Errors { get; set; } = new List<CsvRowReadError>();
    }
}
