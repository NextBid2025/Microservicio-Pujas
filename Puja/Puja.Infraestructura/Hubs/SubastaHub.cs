using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace Puja.Infraestructura.Hubs
{
    public class SubastaHub : Hub
    {
        // Renombrado de "UnirseASubasta" a "JoinSubasta" para que coincida con la llamada del cliente
        public async Task JoinSubasta(string subastaId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, subastaId);
        }
        
        public override async Task OnConnectedAsync()
        {
            await Clients.Caller.SendAsync("ReceiveConnectionId", Context.ConnectionId);
            await base.OnConnectedAsync();
        }
    }
}