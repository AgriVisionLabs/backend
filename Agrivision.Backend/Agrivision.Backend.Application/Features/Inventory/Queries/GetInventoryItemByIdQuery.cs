using Agrivision.Backend.Application.Features.Inventory.Contracts;
using Agrivision.Backend.Domain.Abstractions;
using MediatR;

namespace Agrivision.Backend.Application.Features.Inventory.Queries;

public record GetInventoryItemByIdQuery
(
    Guid FarmId,
    Guid ItemId,
    string RequesterId
) : IRequest<Result<InventoryItemResponse>>;