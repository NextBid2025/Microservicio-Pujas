
using Puja.Domain.Aggregates;

namespace Puja.Domain.Repositories;

public interface IPujaRepository
{
    Task AddAsync(AggregatePuja bid);
    Task<AggregatePuja> GetLastBidAsync(int subastaId);

    
    Task<AggregatePuja?> GetUltimaPujaPorSubastaIdAsync(string subastaId);
    Task<decimal> GetIncrementoMinimoPorSubastaIdAsync(string subastaId);
    Task<decimal> GetPrecioInicialPorSubastaIdAsync(string subastaId);
    Task<AggregatePuja?> GetByIdAsync(string pujaId, CancellationToken cancellationToken);
}