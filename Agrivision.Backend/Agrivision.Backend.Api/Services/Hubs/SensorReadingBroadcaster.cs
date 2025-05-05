using Agrivision.Backend.Api.Hubs;
using Agrivision.Backend.Application.Repositories.Core;
using Agrivision.Backend.Infrastructure.Services.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace Agrivision.Backend.Api.Services.Hubs;

public class SensorReadingBroadcaster(ISensorUnitRepository sensorUnitRepository, IHubContext<SensorHub> hubContext, ILogger<SensorReadingBroadcaster> logger) : ISensorReadingBroadcaster
{
    public async Task BroadcastAsync(Guid unitId, object data)
    {
        var unit = await sensorUnitRepository.FindByIdAsync(unitId);
        if (unit is null)
        {
            logger.LogWarning("Cannot broadcast: unit {UnitId} not found", unitId);
            return;
        }

        await hubContext.Clients.Group($"farm-{unit.FarmId}")
            .SendAsync("ReceiveReading", unitId, data);

        logger.LogInformation("📡 Broadcasted reading from {UnitId} to farm {FarmId}", unitId, unit.FarmId);
    }
}