using Puja.Domain.Entities;

namespace Puja.Domain.Repositories;

public interface IPujaAutomaticaRepository
{
    Task<IEnumerable<PujaAutomaticaConfig>> GetBySubastaIdAsync(string subastaId);
    
}