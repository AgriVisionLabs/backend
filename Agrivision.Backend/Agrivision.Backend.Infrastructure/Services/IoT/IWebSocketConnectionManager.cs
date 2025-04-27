using System.Net.WebSockets;

namespace Agrivision.Backend.Infrastructure.Services.IoT;

public interface IWebSocketConnectionManager
{
    void AddConnection(Guid deviceId, WebSocket socket);
    void RemoveConnection(Guid deviceId);
    WebSocket? GetConnection(Guid deviceId);
    bool IsConnected(Guid deviceId);
    Task<bool> SendPingAsync(Guid deviceId);
    void UpdatePong(Guid deviceId);
    DateTime? GetLastPong(Guid deviceId);
}