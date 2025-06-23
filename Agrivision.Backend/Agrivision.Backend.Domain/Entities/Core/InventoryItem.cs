using System.ComponentModel.DataAnnotations.Schema;
using Agrivision.Backend.Domain.Entities.Shared;
using Agrivision.Backend.Domain.Enums.Core;

namespace Agrivision.Backend.Domain.Entities.Core;

public class InventoryItem : AuditableEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = default!;
    public InventoryItemType Category { get; set; } = InventoryItemType.Fertilizer;
    public float Quantity { get; set; }
    public float ThresholdQuantity { get; set; }
    public float UnitCost { get; set; }
    public string MeasurementUnit { get; set; } = default!;
    public DateTime? ExpirationDate { get; set; }
    
    [NotMapped]
    public int? DaysUntilExpiry =>
        ExpirationDate is null ? null : (ExpirationDate.Value - DateTime.UtcNow).Days;
    
    [NotMapped]
    public string StockLevel
    {
        get
        {
            if (Quantity <= 0 || ThresholdQuantity <= 0)
                return "Low";

            var ratio = Quantity / ThresholdQuantity;

            return ratio switch
            {
                <= 0.75f => "Low",
                <= 1.0f => "Medium",
                _ => "High"
            };
        }
    }
    
    public Guid FarmId { get; set; }
    public Farm Farm { get; set; } = default!;

    public Guid? FieldId { get; set; } // link to a specific field (for location)
    public Field? Field { get; set; }
    
    public ICollection<InventoryItemTransaction> Transactions { get; set; } = new List<InventoryItemTransaction>();
}