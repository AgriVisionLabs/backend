using Agrivision.Backend.Domain.Entities.Shared;
using Agrivision.Backend.Domain.Enums.Core;

namespace Agrivision.Backend.Domain.Entities.Core;

public class Crop : AuditableEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = default!;
    public CropType CropType { get; set; }
    public string? Description { get; set; }
    public int GrowthDurationDays { get; set; }
    public bool SupportsDiseaseDetection { get; set; }
    public List<int> PlantingMonths { get; set; } = new();

    public ICollection<PlantedCrop> PlantedCrops { get; set; } = new List<PlantedCrop>();
}