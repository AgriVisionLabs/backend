using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using Agrivision.Backend.Application.Services.IoT;
using Microsoft.Extensions.Logging;

namespace Agrivision.Backend.Infrastructure.Services.IoT;

public class WebSocketDeviceCommunicator(IWebSocketConnectionManager connectionManager, ILogger<WebSocketDeviceCommunicator> logger) : IWebSocketDeviceCommunicator
{
    public async Task<bool> SendCommandAsync(Guid deviceId, string command)
    {
        var socket = connectionManager.GetConnection(deviceId);

        if (socket is null || socket.State != WebSocketState.Open)
            return false;
        
        try
        {
            var messageObj = new
            {
                type = "command",
                command = command
            };

            var json = JsonSerializer.Serialize(messageObj);
            var buffer = Encoding.UTF8.GetBytes(json);

            await socket.SendAsync(
                new ArraySegment<byte>(buffer),
                WebSocketMessageType.Text,
                true,
                CancellationToken.None);

            return true;
        }
        catch (Exception ex)
        {
            logger.LogError("Error sending command to device {DeviceId}: {Message}", deviceId, ex.Message);
            return false;
        }
    }

    public bool IsDeviceConnected(Guid deviceId)
    {
        return connectionManager.IsConnected(deviceId);
    }
}