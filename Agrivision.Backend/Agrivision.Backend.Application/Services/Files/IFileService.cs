using Microsoft.AspNetCore.Http;

namespace Agrivision.Backend.Application.Services.Files;

public interface IFileService
{
    Task<string> UploadImageAsync(IFormFile file);
    Task<string> UploadVideoAsync(IFormFile file);
    Task<List<string>> ExtractVideoFramesAsync(string videoFilename, int intervalSeconds = 10);
    Task<string> CreateCompositeImageAsync(List<(string imagePath, string label, double confidence)> annotatedFrames, string outputFilename);
}