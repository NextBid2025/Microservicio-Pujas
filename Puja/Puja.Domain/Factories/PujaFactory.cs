namespace Puja.Domain.Factories;

using Puja.Domain.Aggregates;
using Puja.Domain.ValueObjects;

public static class PujaFactory
{
    public static AggregatePuja Create(
        PujaId id,
        SubastaId auctionId,
        UserId userId,
        Monto Monto)
    {
        return new AggregatePuja(
            id,
            auctionId,
            userId,
            Monto
        );
    }
}