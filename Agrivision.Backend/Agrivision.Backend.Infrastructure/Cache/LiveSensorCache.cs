using System.Collections.Concurrent;
using Agrivision.Backend.Domain.Enums.Core;

namespace Agrivision.Backend.Infrastructure.Cache;

public static class LiveSensorCache
{
    private static readonly ConcurrentDictionary<Guid, Dictionary<SensorType, (float Value, DateTime Timestamp)>> _cache = new();

    public static void Update(Guid deviceId, SensorType type, float value)
    {
        var dict = _cache.GetOrAdd(deviceId, _ => new());
        dict[type] = (value, DateTime.UtcNow);
    }

    public static float? TryGetLatest(Guid deviceId, SensorType type, TimeSpan maxAge)
    {
        if (_cache.TryGetValue(deviceId, out var types) &&
            types.TryGetValue(type, out var data) &&
            DateTime.UtcNow - data.Timestamp <= maxAge)
        {
            return data.Value;
        }

        return null;
    }
}