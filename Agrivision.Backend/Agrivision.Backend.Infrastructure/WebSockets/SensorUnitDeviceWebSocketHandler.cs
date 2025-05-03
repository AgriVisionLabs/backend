using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using Agrivision.Backend.Application.Models;
using Agrivision.Backend.Infrastructure.Services.IoT;
using Agrivision.Backend.Infrastructure.Persistence.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Agrivision.Backend.Infrastructure.WebSockets;

public class SensorUnitDeviceWebSocketHandler(IWebSocketConnectionManager connectionManager, IServiceScopeFactory scopeFactory, ILogger<SensorUnitDeviceWebSocketHandler> logger)
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

        var device = await coreDbContext.SensorUnitDevices
            .FirstOrDefaultAsync(d =>
                d.MacAddress == payload.MacAddress && d.ProvisioningKey == payload.ProvisioningKey);
        
        if (device is null)
        {
            await socket.CloseAsync(WebSocketCloseStatus.PolicyViolation, "Unauthorized", CancellationToken.None);
            return;
        }
        
        // var unit = await coreDbContext.IrrigationUnits
        //     .FirstOrDefaultAsync(u => u.DeviceId == device.Id && !u.IsDeleted, CancellationToken.None);
        
        connectionManager.AddConnection(device.Id, socket);
        
        logger.LogInformation("Device connected with MacAddress: {MacAddress}, Ip: {Ip}, and ProvisioningKey: {ProvisioningKey}", payload.MacAddress, payload.Ip, payload.ProvisioningKey);

        device.IsOnline = true;
        device.LastSeen = DateTime.UtcNow;
        
        // if (unit is not null)
        // {
        //     unit.IsOnline = true;
        //     unit.IpAddress = payload.Ip;
        //     unit.LastSeen = DateTime.UtcNow;
        //     
        //     coreDbContext.IrrigationUnits.Update(unit);
        // }
        
        coreDbContext.SensorUnitDevices.Update(device);
        
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
        
        await Listen(socket, device.Id);
    }

    private async Task Listen(WebSocket socket, Guid deviceId)
    {
        var buffer = new byte[1024 * 4];

        while (socket.State == WebSocketState.Open)
        {
            var result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

            if (result.MessageType == WebSocketMessageType.Close && (socket.State == WebSocketState.Open || socket.State == WebSocketState.CloseReceived))
            {
                // if the socket closed naturally without sending disconnect JSON
                logger.LogInformation("Socket closed for deviceId: {DeviceId}", deviceId);
                connectionManager.RemoveConnection(deviceId);
                await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed", CancellationToken.None);
                break;
            }

            if (result.MessageType == WebSocketMessageType.Text)
            {
                var json = Encoding.UTF8.GetString(buffer, 0, result.Count);

                try
                {
                    var payload = JsonSerializer.Deserialize<DeviceMessage>(json);

                    if (payload is not null)
                    {
                        if (payload.Type == "pong")
                        {
                            connectionManager.UpdatePong(deviceId);
                            logger.LogInformation("Pong message received.");
                        }
                        else if (payload.Type == "disconnect" && (socket.State == WebSocketState.Open || socket.State == WebSocketState.CloseReceived))
                        {
                            logger.LogInformation("DeviceId: {DeviceId} sent a disconnect message", deviceId);
                            connectionManager.RemoveConnection(deviceId);
                            await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed", CancellationToken.None);
                            break;
                        } 
                        else if (payload.Type == "ack")
                        {
                            connectionManager.CompleteAck(deviceId, payload.Cid!);
                            logger.LogInformation("Ack for {DeviceId}:{Cid}", deviceId, payload.Cid);
                        }
                        else
                        {
                            logger.LogInformation("Unrecognized message type received.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError("Failed to parse device message: {Message}", ex.Message);
                }
            }
        }
    }
}