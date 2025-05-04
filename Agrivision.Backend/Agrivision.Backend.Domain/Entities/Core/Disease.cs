
using Agrivision.Backend.Domain.Entities.Shared;

namespace Agrivision.Backend.Domain.Entities.Core;
public class Disease : AuditableEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public Guid CropTypeId { get; set; }
    public CropType CropType { get; set; } = default!;

    public int ClassIdInModelPredictions {  get; set; } // exist also in prediction object recivced from model
    public bool Is_Safe { get; set; }
    public ICollection<DiseaseDetection> DiseaseDetections { get; set; } = [];
}
