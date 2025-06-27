using Agrivision.Backend.Domain.Entities.Shared;
using Agrivision.Backend.Domain.Enums.Core;

namespace Agrivision.Backend.Domain.Entities.Core;

public class Field : AuditableEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public double Area { get; set; }
    public bool IsActive { get; set; } // could use the FieldStatus and add more than two (e.g., active, inactive, pending, etc)
    
    public Guid? PlantedCropId { get; set; }
    public PlantedCrop? PlantedCrop { get; set; }
    
    // navigational property
    public Guid FarmId { get; set; }
    public Farm Farm { get; set; } = default!;

    public IrrigationUnit IrrigationUnit { get; set; } = default!;
    public ICollection<SensorUnit> SensorUnits { get; set; } = new List<SensorUnit>();
    public ICollection<TaskItem> TaskItems { get; set; } = new List<TaskItem>();
    public ICollection<InventoryItem> InventoryItems { get; set; } = new List<InventoryItem>();
}