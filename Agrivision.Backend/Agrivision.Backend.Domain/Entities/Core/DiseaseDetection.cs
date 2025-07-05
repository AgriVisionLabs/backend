using Agrivision.Backend.Domain.Entities.Shared;
using Agrivision.Backend.Domain.Enums.Core;

namespace Agrivision.Backend.Domain.Entities.Core;

public class DiseaseDetection : AuditableEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    public double ConfidenceLevel { get; set; }
    public string ImageUrl { get; set; } = default!;
    public bool IsHealthy { get; set; }
    public PlantHealthStatus HealthStatus { get; set; } = PlantHealthStatus.Healthy;

    public Guid PlantedCropId { get; set; }
    public PlantedCrop? PlantedCrop { get; set; } = default!;

    public Guid? CropDiseaseId { get; set; }
    public CropDisease? CropDisease { get; set; }
}