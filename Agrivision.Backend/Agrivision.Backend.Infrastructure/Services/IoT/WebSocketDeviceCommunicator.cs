using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using Agrivision.Backend.Application.Services.IoT;
using Microsoft.Extensions.Logging;

namespace Agrivision.Backend.Infrastructure.Services.IoT;

public class WebSocketDeviceCommunicator(IWebSocketConnectionManager connectionManager, ILogger<WebSocketDeviceCommunicator> logger) : IWebSocketDeviceCommunicator
{
    public async Task<bool> SendCommandAsync(Guid deviceId, string command, CancellationToken cancellationToken = default)
    {
        var socket = connectionManager.GetConnection(deviceId);
        if (socket is null || socket.State != WebSocketState.Open)
            return false;

        var cid = Guid.NewGuid().ToString("N");

        var ackTask = connectionManager.RegisterAckWaiter(deviceId, cid,
            TimeSpan.FromSeconds(15), cancellationToken);

        var payload = JsonSerializer.Serialize(new
        {
            type = "command",
            cmd  = command,
            cid
        });
        var buffer = Encoding.UTF8.GetBytes(payload);

        await socket.SendAsync(buffer, WebSocketMessageType.Text, true, cancellationToken);

        return await ackTask;
    }

    public bool IsDeviceConnected(Guid deviceId)
    {
        return connectionManager.IsConnected(deviceId);
    }

    public async Task<bool> SendConfirmationAsync(Guid deviceId, string command)
    {
        var socket = connectionManager.GetConnection(deviceId);

        if (socket is null || socket.State != WebSocketState.Open)
            return false;
        
        try
        {
            var messageObj = new
            {
                type = "ack",
                command = command,
                message = "Acknowledgment received successfully to Agrivision Server. "
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
}