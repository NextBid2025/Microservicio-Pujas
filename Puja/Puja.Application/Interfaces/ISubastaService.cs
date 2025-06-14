namespace Puja.Application.Interfaces;

public interface ISubastaService
{
    Task<bool> SubastaEstaActivaAsync(string subastaId);
}