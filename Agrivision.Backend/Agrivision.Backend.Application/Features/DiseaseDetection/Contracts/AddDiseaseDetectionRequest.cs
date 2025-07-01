using Microsoft.AspNetCore.Http;

namespace Agrivision.Backend.Application.Features.DiseaseDetection.Contracts;

public class AddDiseaseDetectionRequest
{
    public IFormFile Image { get; set; } = default!;
}