namespace Puja.Application.Interfaces;

public interface INotificacionPujaService
{
    Task NotificarNuevaPujaAsync(string subastaId, object datosPuja);
    Task NotificarPujaAutomaticaAsync(string userId, string subastaId, decimal monto);
    Task NotificarLimiteAlcanzadoAsync(string userId, string subastaId);
    
    public Task NotificarPujaInvalidaAsync(string subastaId, string mensaje);
}