namespace Puja.Domain.ValueObjects;

public class SubastaId
{
    public string Value { get; private set; }

    public SubastaId(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("El ID de la subasta no puede estar vacío.");

        Value = value;
    }

    public override string ToString() => Value.ToString();
}
