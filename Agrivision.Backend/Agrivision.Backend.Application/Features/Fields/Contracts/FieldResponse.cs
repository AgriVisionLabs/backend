using Agrivision.Backend.Domain.Enums.Core;

namespace Agrivision.Backend.Application.Features.Fields.Contracts;

public record FieldResponse
(
    Guid Id,
    string Name,
    double Area,
    bool IsActive,
    Guid FarmId,
    CropType? CropType,
    string? CropName,
    string? Description,
    bool? SupportsDiseaseDetection,
    DateTime? PlantingDate,
    int? Progress,
    DateTime? ExpectedHarvestDate
);