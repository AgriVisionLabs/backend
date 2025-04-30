namespace Agrivision.Backend.Application.Services.IoT;

public interface IWebSocketDeviceCommunicator
{
    Task<bool> SendCommandAsync(Guid deviceId, string command);
    bool IsDeviceConnected(Guid deviceId);
    public DateTime? GetLastAck(Guid deviceId, string command);
    Task<bool> SendConfirmationAsync(Guid deviceId);
}