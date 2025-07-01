using Agrivision.Backend.Domain.Entities.Shared;

namespace Agrivision.Backend.Domain.Entities.Core;

public class PlantedCrop : AuditableEntity
{
    public Guid Id { get; set; }
    public DateTime PlantingDate { get; set; }
    public DateTime? ExpectedHarvestDate { get; set; }
    public DateTime? ActualHarvestDate { get; set; }
    public double? Yield { get; set; }

    public Guid CropId { get; set; }
    public Crop Crop { get; set; } = default!;

    public Guid FieldId { get; set; }
    public Field Field { get; set; } = default!;

    public ICollection<DiseaseDetection> DiseaseDetections = new List<DiseaseDetection>();
}