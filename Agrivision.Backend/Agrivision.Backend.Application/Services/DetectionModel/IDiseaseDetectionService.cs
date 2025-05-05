

namespace Agrivision.Backend.Application.Services.DetectionModel;
public interface IDiseaseDetectionService
{
    Task<string> NewDetectionAsync(string imagePath);
}
