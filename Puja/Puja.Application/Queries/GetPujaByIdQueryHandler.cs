using System.Threading;
using System.Threading.Tasks;
using BluidingBlocks.CQRS;

using Puja.Application.DTOS;
using Puja.Domain.Repositories;

namespace Puja.Application.Queries;

public class GetPujaByIdQueryHandler : IQueryHandler<GetPujaByIdQuery, PujaDto>
{
    private readonly IPujaRepository _pujaRepository;

    public GetPujaByIdQueryHandler(IPujaRepository pujaRepository)
    {
        _pujaRepository = pujaRepository;
    }

    public async Task<PujaDto> Handle(GetPujaByIdQuery request, CancellationToken cancellationToken)
    {
        var puja = await _pujaRepository.GetByIdAsync(request.PujaId, cancellationToken);
        if (puja == null)
            return null;

        return new PujaDto(
            puja.Id.Value.ToString(),
            puja.Id.Value.ToString(),
            puja.UserId.Value,
            puja.Monto.Value,
            puja.FechaPuja
        );
    }
}