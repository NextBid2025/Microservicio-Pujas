using BluidingBlocks.CQRS;

using Puja.Application.DTOS;

namespace Puja.Application.Queries;

public class GetPujaByIdQuery : IQuery<PujaDto>
{
    public string PujaId { get; }

    public GetPujaByIdQuery(string pujaId)
    {
        PujaId = pujaId;
    }
}