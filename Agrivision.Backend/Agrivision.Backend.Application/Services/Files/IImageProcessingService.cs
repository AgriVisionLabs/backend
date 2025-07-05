namespace Agrivision.Backend.Application.Services.Files;

public interface IImageProcessingService
{
    Task<string> CreateCompositeImageAsync(List<(string imagePath, string label, double confidence)> annotatedFrames, string outputFilename);
} 