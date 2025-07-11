using Microsoft.AspNetCore.SignalR;

using Puja.Application.Interfaces;
using Puja.Infraestructura.Hubs;

namespace Puja.Infraestructura.Services;

public class NotificacionPujaService : INotificacionPujaService
{
    private readonly IHubContext<SubastaHub> _hubContext;

    public NotificacionPujaService(IHubContext<SubastaHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task NotificarNuevaPujaAsync(string subastaId, object datosPuja)
    {
        await _hubContext.Clients.Group(subastaId).SendAsync("NuevaPuja", datosPuja);
    }
    
    public async Task NotificarLimiteAlcanzadoAsync(string userId, string mensaje)
    {
        await _hubContext.Clients.User(userId).SendAsync("LimiteAlcanzado", mensaje);
    }

    public async Task NotificarPujaAutomaticaAsync(string userId, string subastaId, decimal monto)
    {
        await _hubContext.Clients.User(userId).SendAsync("PujaAutomatica", new { SubastaId = subastaId, Monto = monto });
    }
}