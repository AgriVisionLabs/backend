using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Agrivision.Backend.Api.Hubs;

[Authorize]
public class SensorHub : Hub
{
    public async Task SendReading(Guid deviceId, object data)
    {
        await Clients.All.SendAsync("ReceiveReading", deviceId, data);
    }
    
    public async Task Echo(string message)
    {
        await Clients.Caller.SendAsync("EchoResponse", $"Server heard you say: {message}");
    }
}