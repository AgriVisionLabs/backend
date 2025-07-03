using Agrivision.Backend.Domain.Entities.Shared;

namespace Agrivision.Backend.Domain.Entities.Core;

public class DiseaseDetection : AuditableEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    public double ConfidenceLevel { get; set; }
    public string ImageUrl { get; set; } = default!;

    public Guid PlantedCropId { get; set; }
    public PlantedCrop PlantedCrop { get; set; } = default!;

    public Guid? CropDiseaseId { get; set; }
    public CropDisease? CropDisease { get; set; }
}