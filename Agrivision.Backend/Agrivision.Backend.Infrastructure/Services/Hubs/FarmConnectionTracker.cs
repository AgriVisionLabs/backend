using System.Collections.Concurrent;
using Agrivision.Backend.Application.Services.Hubs;

namespace Agrivision.Backend.Infrastructure.Services.Hubs;

public class FarmConnectionTracker : IFarmConnectionTracker
{
    private readonly ConcurrentDictionary<string, Guid> _connections = new();

    public void Add(string connectionId, Guid farmId)
    {
        _connections[connectionId] = farmId;
    }

    public bool TryRemove(string connectionId, out Guid farmId)
    {
        return _connections.TryRemove(connectionId, out farmId);
    }
}