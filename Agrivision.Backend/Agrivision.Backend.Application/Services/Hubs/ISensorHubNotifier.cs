namespace Agrivision.Backend.Application.Services.Hubs;

public interface ISensorHubNotifier
{
    Task AddConnectionToFarmGroup(string connectionId, Guid farmId);
    Task RemoveConnectionFromFarmGroup(string connectionId, Guid farmId);
}