using Agrivision.Backend.Domain.Entities.Shared;
using Agrivision.Backend.Domain.Enums.Core;

namespace Agrivision.Backend.Domain.Entities.Core;

public class InventoryItemTransaction : AuditableEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid InventoryItemId { get; set; }
    public InventoryItem InventoryItem { get; set; } = default!;

    public float QuantityChanged { get; set; } // positive = added, negative = used
    public InventoryTransactionType Reason { get; set; } = InventoryTransactionType.InitialStock;
}