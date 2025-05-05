namespace Agrivision.Backend.Infrastructure.Services.Hubs;

public interface ISensorReadingBroadcaster
{
    Task BroadcastAsync(Guid unitId, object data);
}