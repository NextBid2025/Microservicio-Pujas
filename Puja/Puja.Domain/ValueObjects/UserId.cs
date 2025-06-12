namespace Puja.Domain.ValueObjects;

public class UserId
{
    public string Value { get; private set; }

    public UserId(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("El ID del usuario no puede estar vacío.");

        Value = value;
    }

    public override string ToString() => Value.ToString();
}
