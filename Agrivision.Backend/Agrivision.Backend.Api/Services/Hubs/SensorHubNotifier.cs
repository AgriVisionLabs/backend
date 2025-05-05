using Agrivision.Backend.Api.Hubs;
using Agrivision.Backend.Application.Services.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace Agrivision.Backend.Api.Services.Hubs;

public class SensorHubNotifier : ISensorHubNotifier
{
    private readonly IHubContext<SensorHub> _hubContext;

    public SensorHubNotifier(IHubContext<SensorHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task AddConnectionToFarmGroup(string connectionId, Guid farmId)
    {
        await _hubContext.Groups.AddToGroupAsync(connectionId, $"farm-{farmId}");
    }
    
    public Task RemoveConnectionFromFarmGroup(string connectionId, Guid farmId)
    {
        return _hubContext.Groups.RemoveFromGroupAsync(connectionId, $"farm-{farmId}");
    }
}