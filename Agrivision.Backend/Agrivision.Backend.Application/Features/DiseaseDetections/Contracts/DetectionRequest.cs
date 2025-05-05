

using Microsoft.AspNetCore.Http;

namespace Agrivision.Backend.Application.Features.DiseaseDetections.Contracts;
public record DetectionRequest
(
    IFormFile Image 
);