using System;

namespace CubeCsv
{
    public class CsvFieldSchema
    {
        public string Name { get; set; }
        public Type Type { get; set; }
        public int Length { get; set; } = 0;
        public CsvFieldValidator Validator { get; set; }

        public CsvFieldSchema() { }
        public CsvFieldSchema(string name)
        {
            Name = name;
        }
        public CsvFieldSchema(Type type)
        {
            Type = type;
        }
        public CsvFieldSchema(string name, Type type, int length = 0, CsvFieldValidator validator = null)
        {
            Name = name;
            Type = type;
            Length = length;
            Validator = validator;
        }
        public override bool Equals(object target)
        {
            CsvFieldSchema schema = target as CsvFieldSchema;
            if (schema != null)
                return Type == schema.Type && Name == schema.Name && (Length == schema.Length || Length == 0);
            return false;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        public override string ToString()
        {
            return Name;
        }
    }
}
