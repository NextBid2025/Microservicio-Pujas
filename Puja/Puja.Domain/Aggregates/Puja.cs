using Puja.Domain.ValueObjects;

namespace Puja.Domain.Aggregates;



public class AggregatePuja
{
    public PujaId Id { get; private set; }
    public SubastaId AuctionId { get; private set; }
    public UserId UserId { get; private set; }
    public Monto Monto { get; private set; }
    public DateTime FechaPuja { get; private set; }

    private AggregatePuja() { }

    // Constructor principal
    public AggregatePuja(
        PujaId id,
        SubastaId auctionId,
        UserId userId,
        Monto amount
    )
    {
        Id = id;
        AuctionId = auctionId;
        UserId = userId;
        Monto = amount;
        FechaPuja = DateTime.UtcNow;
    }

    public void UpdatePuja(decimal? amount, string? userId, string? auctionId)
    {
        if (amount != null)
        {
            this.Monto = new Monto(amount.Value);
            this.FechaPuja = DateTime.UtcNow;
        }
        if (userId != null)
        {
            this.UserId = new UserId(userId);
        }
        if (auctionId != null)
        {
            this.AuctionId = new SubastaId(auctionId);
        }
    }
}