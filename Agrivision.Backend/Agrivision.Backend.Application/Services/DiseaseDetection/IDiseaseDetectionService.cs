using Agrivision.Backend.Domain.Models;

namespace Agrivision.Backend.Application.Services.DiseaseDetection;

public interface IDiseaseDetectionService
{
    Task<DiseasePredictionResponse> PredictImageAsync(string filename);
    // Task<string> PredictVideoAsync(byte[] videoBytes);
}