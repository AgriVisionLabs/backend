using System.Net.WebSockets;
using System.Collections.Concurrent;

namespace Agrivision.Backend.Infrastructure.Services.IoT;

public class WebSocketConnectionManager : IWebSocketConnectionManager
{
    private readonly ConcurrentDictionary<Guid, WebSocket> _connections = new();
    
    public void AddConnection(Guid deviceId, WebSocket socket)
    {
        _connections[deviceId] = socket;
    }

    public void RemoveConnection(Guid deviceId)
    {
        _connections.TryRemove(deviceId, out _);
    }

    public WebSocket? GetConnection(Guid deviceId)
    {
        return _connections.TryGetValue(deviceId, out var socket) ? socket : null;
    }

    public bool IsConnected(Guid deviceId)
    {
        return _connections.ContainsKey(deviceId);
    }
    
}