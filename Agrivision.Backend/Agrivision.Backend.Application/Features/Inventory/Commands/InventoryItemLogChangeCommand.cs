using Agrivision.Backend.Domain.Abstractions;
using Agrivision.Backend.Domain.Enums.Core;
using MediatR;

namespace Agrivision.Backend.Application.Features.Inventory.Commands;

public record InventoryItemLogChangeCommand
(
    Guid FarmId,
    Guid ItemId,
    string RequesterId,
    float Quantity,
    InventoryTransactionType Reason
) : IRequest<Result>;