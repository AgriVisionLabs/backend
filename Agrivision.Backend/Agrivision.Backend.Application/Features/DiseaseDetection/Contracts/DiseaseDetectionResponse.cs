namespace Agrivision.Backend.Application.Features.DiseaseDetection.Contracts;

public record DiseaseDetectionResponse
(
    Guid Id,
    Guid FarmId,
    Guid FieldId,
    string CropName,
    string DiseaseName,
    bool IsHealthy,
    DateTime CreatedOn,
    double ConfidenceLevel,
    string ImageUrl,
    string CreatedBy,
    List<string> Treatments
);