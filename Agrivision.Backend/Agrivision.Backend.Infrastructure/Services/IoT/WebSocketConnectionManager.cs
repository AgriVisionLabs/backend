using System.Net.WebSockets;
using System.Collections.Concurrent;
using System.Text;
using System.Text.Json;

namespace Agrivision.Backend.Infrastructure.Services.IoT;

public class WebSocketConnectionManager : IWebSocketConnectionManager
{
    private readonly ConcurrentDictionary<Guid, WebSocket> _connections = new();
    private readonly ConcurrentDictionary<Guid, DateTime> _lastPong = new();
    private readonly ConcurrentDictionary<string, TaskCompletionSource<bool>> _pending = new(StringComparer.Ordinal);
    
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
    
    public async Task<bool> SendPingAsync(Guid deviceId)
    {
        var socket = GetConnection(deviceId);

        if (socket is null || socket.State != WebSocketState.Open)
            return false;

        var pingMessage = JsonSerializer.Serialize(new { type = "ping" });
        var buffer = Encoding.UTF8.GetBytes(pingMessage);

        try
        {
            await socket.SendAsync(
                new ArraySegment<byte>(buffer),
                WebSocketMessageType.Text,
                true,
                CancellationToken.None);

            return true;
        }
        catch
        {
            return false;
        }
    }
    
    public void UpdatePong(Guid deviceId)
    {
        _lastPong[deviceId] = DateTime.UtcNow;
    }
    
    public DateTime? GetLastPong(Guid deviceId)
    {
        if (_lastPong.TryGetValue(deviceId, out var lastPong))
        {
            return lastPong;
        }

        return null;
    }
    
    public Task<bool> RegisterAckWaiter(Guid deviceId, string cid, TimeSpan timeout, CancellationToken cancellationToken = default)
    {
        var key = $"{deviceId}:{cid}";
        var tcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        _pending[key] = tcs;

        // auto-cancel after timeout
        var timeoutCts = new CancellationTokenSource(timeout);
        var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutCts.Token);
        linkedCts.Token.Register(() => {
            if (_pending.TryRemove(key, out var source))
            {
                source.TrySetResult(false);
            }
        });

        return tcs.Task;
    }
    
    public void CompleteAck(Guid deviceId, string cid)
    {
        var key = $"{deviceId}:{cid}";
        if (_pending.TryRemove(key, out var tcs))
            tcs.TrySetResult(true);
    }
}