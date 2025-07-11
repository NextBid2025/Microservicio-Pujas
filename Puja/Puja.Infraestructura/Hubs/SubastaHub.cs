using Microsoft.AspNetCore.SignalR;

namespace Puja.Infraestructura.Hubs;

public class SubastaHub : Hub
{
    public async Task UnirseASubasta(string subastaId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, subastaId);
    }
}