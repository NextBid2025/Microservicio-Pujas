namespace Puja.Domain.ValueObjects
{
    public class PujaId
    {
        public string Value { get; private set; }

        public PujaId(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("El ID de la puja no puede estar vacío.");

            Value = value;
        }

        public override string ToString() => Value.ToString();
    }
}