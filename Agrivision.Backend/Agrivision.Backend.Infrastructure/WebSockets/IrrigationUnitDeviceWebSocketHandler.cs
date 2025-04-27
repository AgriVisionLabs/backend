using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using Agrivision.Backend.Application.Features.WebSockets;
using Agrivision.Backend.Infrastructure.Services.IoT;
using Agrivision.Backend.Infrastructure.Persistence.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Agrivision.Backend.Infrastructure.WebSockets;

public class IrrigationUnitDeviceWebSocketHandler(IWebSocketConnectionManager connectionManager, IServiceScopeFactory scopeFactory, ILogger<IrrigationUnitDeviceWebSocketHandler> logger)
{
    public async Task HandleAsync(HttpContext context)
    {
        if (!context.WebSockets.IsWebSocketRequest)
        {
            context.Response.StatusCode = 400;
            return;
        }

        var socket = await context.WebSockets.AcceptWebSocketAsync();

        using var scope = scopeFactory.CreateScope();
        var coreDbContext = scope.ServiceProvider.GetRequiredService<CoreDbContext>();

        var buffer = new byte[1024 * 4];
        var result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
        var json = Encoding.UTF8.GetString(buffer, 0, result.Count);
        var payload = JsonSerializer.Deserialize <DeviceMessage>(json);

        if (payload is null)
        {
            await socket.CloseAsync(WebSocketCloseStatus.PolicyViolation, "Invalid message", CancellationToken.None);
            return;
        }

        var device = await coreDbContext.IrrigationUnitDevices
            .FirstOrDefaultAsync(d =>
                d.MacAddress == payload.MacAddress && d.ProvisioningKey == payload.ProvisioningKey);

        if (device is null)
        {
            await socket.CloseAsync(WebSocketCloseStatus.PolicyViolation, "Unauthorized", CancellationToken.None);
            return;
        }
        
        connectionManager.AddConnection(device.Id, socket);
        
        logger.LogInformation("Device connected with MacAddress: {MacAddress}, Ip: {Ip}, and ProvisioningKey: {ProvisioningKey}", payload.MacAddress, payload.Ip, payload.ProvisioningKey);

        device.IsOnline = true;
        coreDbContext.IrrigationUnitDevices.Update(device);
        await coreDbContext.SaveChangesAsync(CancellationToken.None);
        
        var confirmationMessage = new
        {
            type = "connected",
            message = "Device successfully connected to Agrivision Server"
        };

        var confirmationJson = JsonSerializer.Serialize(confirmationMessage);
        var confirmationBuffer = Encoding.UTF8.GetBytes(confirmationJson);

        await socket.SendAsync(
            new ArraySegment<byte>(confirmationBuffer),
            WebSocketMessageType.Text,
            true,
            CancellationToken.None);

        
        
        var unit = await coreDbContext.IrrigationUnits
            .FirstOrDefaultAsync(u => u.DeviceId == device.Id && !u.IsDeleted, CancellationToken.None);
        if (unit is not null)
        {
            unit.IsOnline = true;
            unit.IpAddress = payload.Ip;
            unit.LastSeen = DateTime.UtcNow;
            
            coreDbContext.IrrigationUnits.Update(unit);
            await coreDbContext.SaveChangesAsync(CancellationToken.None);
        }
        
        await Listen(socket, device.Id);
    }

    private async Task Listen(WebSocket socket, Guid deviceId)
    {
        var buffer = new byte[1024 * 4];

        while (socket.State == WebSocketState.Open)
        {
            var result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

            if (result.MessageType == WebSocketMessageType.Close)
            {
                connectionManager.RemoveConnection(deviceId);
                await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed", CancellationToken.None);
            }
        }
    }
}