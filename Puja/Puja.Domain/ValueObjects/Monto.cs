namespace Puja.Domain.ValueObjects;

public class Monto
{
    public decimal Value { get; private set; }

    public Monto(decimal value)
    {
        if (value < 0)
            throw new ArgumentException("El monto no puede ser negativo.");

        Value = value;
    }

    public override string ToString() => Value.ToString("F2");
}