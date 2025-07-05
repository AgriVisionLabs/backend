using Agrivision.Backend.Domain.Models;

namespace Agrivision.Backend.Application.Services.DiseaseDetection;

public interface IDiseaseDetectionService
{
    Task<DiseasePredictionResponse?> PredictImageAsync(string filename);
    Task<(string compositeImageUrl, int healthyCount, int infectedCount)> PredictVideoAsync(string filename);
}