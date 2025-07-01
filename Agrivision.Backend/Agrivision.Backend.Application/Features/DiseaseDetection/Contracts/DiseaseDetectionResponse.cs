namespace Agrivision.Backend.Application.Features.DiseaseDetection.Contracts;

public record DiseaseDetectionResponse
(
    Guid Id,
    string DiseaseName,
    bool IsHealthy,
    DateTime CreatedOn,
    double ConfidenceLevel,
    string ImageUrl,
    string CreatedBy,
    List<string> Treatments
);