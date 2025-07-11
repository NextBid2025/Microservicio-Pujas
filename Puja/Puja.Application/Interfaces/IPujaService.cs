namespace Puja.Application.Interfaces
{
    public interface IPujaService
    {
        Task CrearPujaAutomaticaAsync(string userId, string subastaId, decimal monto);
       
    }
}