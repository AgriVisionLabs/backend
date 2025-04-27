using System.Net.WebSockets;
using Agrivision.Backend.Infrastructure.Persistence.Core;
using Agrivision.Backend.Infrastructure.Services.IoT;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Agrivision.Backend.Infrastructure.WebSockets;

public class IrrigationUnitDeviceHeartbeatService(IServiceScopeFactory scopeFactory, IWebSocketConnectionManager connectionManager, ILogger<IrrigationUnitDeviceHeartbeatService> logger) : BackgroundService
{
    private readonly Dictionary<Guid, int> _failedPongs = new();
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                // check heartbeat status
                await CheckDeviceStatus();
            }
            catch (Exception ex)
            {
                // log errors but don't crash the background service
                logger.LogError("Error during heartbeat check: {Message}", ex.Message);
            }

            // sleep for a while before next round
            await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
        }
    }

    private async Task CheckDeviceStatus()
    {
        using var scope = scopeFactory.CreateScope();
        var coreDbContext = scope.ServiceProvider.GetRequiredService<CoreDbContext>();

        var devices = await coreDbContext.IrrigationUnitDevices
            .Where(d => !d.IsDeleted)
            .ToListAsync();

        var unitsByDeviceId = await coreDbContext.IrrigationUnits
            .Where(u => !u.IsDeleted)
            .ToDictionaryAsync(u => u.DeviceId, cancellationToken: CancellationToken.None);

        foreach (var device in devices)
        {
            if (!connectionManager.IsConnected(device.Id))
            {
                // not even connected
                if (device.IsOnline)
                {
                    device.IsOnline = false;
                    device.LastSeen = DateTime.UtcNow;
                }
                
                _failedPongs.Remove(device.Id);
                
                continue;
            }
            
            // send ping to the device
            await connectionManager.SendPingAsync(device.Id);
            
            var lastPong = connectionManager.GetLastPong(device.Id);

            if (lastPong is null || (DateTime.UtcNow - lastPong.Value).TotalSeconds > 30)
            {
                if (!_failedPongs.TryAdd(device.Id, 1))
                    _failedPongs[device.Id]++;

                if (_failedPongs[device.Id] >= 6)
                {
                    var socket = connectionManager.GetConnection(device.Id);
                    if (socket is not null && (socket.State == WebSocketState.Open || socket.State == WebSocketState.CloseReceived))
                    {
                        await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Missed too many pongs", CancellationToken.None);
                    }

                    connectionManager.RemoveConnection(device.Id);
                    _failedPongs.Remove(device.Id);

                    // mark device offline if needed
                    if (device.IsOnline)
                    {
                        device.IsOnline = false;
                        device.LastSeen = DateTime.UtcNow;
                    }
                    
                    logger.LogInformation("The device with the id: {DeviceId} was disconnected due to too many missed pongs.", device.Id);

                    continue;
                }

                // device still missing pongs, don't trust it.
                if (device.IsOnline)
                {
                    device.IsOnline = false;
                    device.LastSeen = DateTime.UtcNow;
                }
            }
            // sync irrigation unit too
            if (unitsByDeviceId.TryGetValue(device.Id, out var unit))
            {
                unit.IsOnline = device.IsOnline;
                unit.LastSeen = device.LastSeen;
            }
        }
        
        // save changes for the devices that weren't even connected
        if (coreDbContext.ChangeTracker.HasChanges())
        {
            await coreDbContext.SaveChangesAsync();
        }
    }
}