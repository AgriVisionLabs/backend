using Agrivision.Backend.Domain.Enums.Core;

namespace Agrivision.Backend.Application.Features.Inventory.Contracts;

public record InventoryItemLogChangeRequest
(
    float Quantity,
    InventoryTransactionType Reason
);