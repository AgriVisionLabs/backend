using Agrivision.Backend.Domain.Entities.Shared;
using Agrivision.Backend.Domain.Enums.Core;

namespace Agrivision.Backend.Domain.Entities.Core;

public class Farm : AuditableEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public double Area { get; set; }
    public string Location { get; set; }
    public SoilType SoilType { get; set; }
    public int FieldsNo { get; set; }
    
    // navigational properties
    public ICollection<Field> Fields { get; set; } = new List<Field>();
    public ICollection<FarmUserRole> FarmUserRoles { get; set; } = new List<FarmUserRole>();
    public ICollection<FarmInvitation> FarmInvitations { get; set; } = new List<FarmInvitation>();
    public ICollection<IrrigationUnit> IrrigationUnits { get; set; } = new List<IrrigationUnit>();
    public ICollection<SensorUnit> SensorUnits { get; set; } = new List<SensorUnit>();
    public ICollection<AutomationRule> AutomationRules { get; set; } = new List<AutomationRule>();
    public ICollection<InventoryItem> InventoryItems { get; set; } = new List<InventoryItem>();
    public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
}