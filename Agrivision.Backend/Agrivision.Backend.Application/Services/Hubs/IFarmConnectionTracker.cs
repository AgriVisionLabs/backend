namespace Agrivision.Backend.Application.Services.Hubs;

public interface IFarmConnectionTracker
{
    void Add(string connectionId, Guid farmId);
    bool TryRemove(string connectionId, out Guid farmId);
}