namespace Agrivision.Backend.Application.Services.Files;

public interface IVideoProcessingService
{
    Task<List<string>> ExtractVideoFramesAsync(string videoFilename, int intervalSeconds = 10);
} 