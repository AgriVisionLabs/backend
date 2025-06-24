using Agrivision.Backend.Domain.Enums.Core;

namespace Agrivision.Backend.Application.Features.Inventory.Contracts;

public record UpdateInventoryItemRequest
(
    Guid? FieldId,
    string Name,
    InventoryItemType Category,
    float Quantity,
    float ThresholdQuantity,
    float UnitCost,
    string MeasurementUnit,
    DateTime? ExpirationDate        
);