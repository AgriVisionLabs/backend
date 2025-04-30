namespace Agrivision.Backend.Application.Services.IoT;

public interface IWebSocketDeviceCommunicator
{
    Task<bool> SendCommandAsync(Guid deviceId, string command, CancellationToken cancellationToken = default);
    bool IsDeviceConnected(Guid deviceId);
    Task<bool> SendConfirmationAsync(Guid deviceId, string command);
}