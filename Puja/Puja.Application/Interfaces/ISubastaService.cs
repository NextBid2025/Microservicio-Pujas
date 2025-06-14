namespace Puja.Application.Interfaces;

public interface ISubastaService
{
    Task<bool> SubastaEstaActivaAsync(string subastaId);
    Task ActualizarPrecioSubastaAsync(string subastaId, decimal nuevoPrecio);
}