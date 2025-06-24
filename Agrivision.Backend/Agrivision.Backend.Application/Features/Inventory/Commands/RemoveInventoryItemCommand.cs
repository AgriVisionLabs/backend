using Agrivision.Backend.Domain.Abstractions;
using MediatR;

namespace Agrivision.Backend.Application.Features.Inventory.Commands;

public record RemoveInventoryItemCommand
(
    Guid FarmId,
    Guid ItemId,
    string RequesterId
) : IRequest<Result>;