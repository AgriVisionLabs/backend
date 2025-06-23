using Agrivision.Backend.Domain.Enums.Core;

namespace Agrivision.Backend.Application.Features.Inventory.Contracts;

public record InventoryItemResponse
(
    Guid Id,
    Guid FarmId,
    Guid? FieldId,
    string CreatedById,
    string Name,
    InventoryItemType Category,
    float Quantity,
    float ThresholdQuantity,
    float UnitCost,
    string MeasurementUnit,
    DateTime? ExpirationDate,
    int? DayTillExpiry,
    string StockLevel
);