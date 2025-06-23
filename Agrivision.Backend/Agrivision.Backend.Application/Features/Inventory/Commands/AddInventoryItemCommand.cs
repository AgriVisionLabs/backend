using Agrivision.Backend.Application.Features.Inventory.Contracts;
using Agrivision.Backend.Domain.Abstractions;
using Agrivision.Backend.Domain.Enums.Core;
using MediatR;

namespace Agrivision.Backend.Application.Features.Inventory.Commands;

public record AddInventoryItemCommand
(
    Guid FarmId,
    Guid? FieldId,
    string RequesterId,
    string Name,
    InventoryItemType Category,
    float Quantity,
    float ThresholdQuantity,
    float UnitCost,
    string MeasurementUnit,
    DateTime? ExpirationDate
) : IRequest<Result<InventoryItemResponse>>;