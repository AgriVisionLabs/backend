using Agrivision.Backend.Domain.Enums.Core;

namespace Agrivision.Backend.Application.Features.Inventory.Contracts;

public record InventoryItemTransactionResponse
(
    Guid Id,
    Guid InventoryItemId,
    string InventoryItemName,
    InventoryItemType ItemCategory,
    float UnitCost,
    string MeasurementUnit,
    DateTime TransactionDate,
    float QuantityChanged,
    InventoryTransactionType Reason
);