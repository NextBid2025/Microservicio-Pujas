using Puja.Domain.ValueObjects;

namespace Puja.Application.DTOS;

public record PujaDto(
    string PujaId,
    string SubastaId,
    string UserId,
    decimal Monto,
    DateTime FechaPuja
   
);