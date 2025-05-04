using Agrivision.Backend.Domain.Entities.Shared;
using Agrivision.Backend.Domain.Enums.Core;

namespace Agrivision.Backend.Domain.Entities.Core;

public class Field : AuditableEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public double Area { get; set; }
    public bool IsActive { get; set; } // could use the FieldStatus and add more than two (e.g., active, inactive, pending, etc)
    
    // navigational property
    public Guid FarmId { get; set; }
    public Farm Farm { get; set; } = default!;

    public Guid CropTypeId { get; set; }
    public CropType CropType { get; set; }

    public ICollection<IrrigationUnit> IrrigationUnits { get; set; } = new List<IrrigationUnit>();
    public ICollection<DiseaseDetection> DiseaseDetection { get; set; } = [];

}