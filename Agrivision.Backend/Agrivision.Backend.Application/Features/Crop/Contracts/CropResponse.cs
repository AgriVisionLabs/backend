using Agrivision.Backend.Domain.Enums.Core;

namespace Agrivision.Backend.Application.Features.Crop.Contracts;

public record CropResponse
(
    Guid Id,
    string Name,
    CropType CropType,
    SoilType SoilType,
    bool SupportsDiseaseDetection,
    bool Recommended
);