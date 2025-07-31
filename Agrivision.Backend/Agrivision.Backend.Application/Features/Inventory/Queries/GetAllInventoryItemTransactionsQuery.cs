using Agrivision.Backend.Application.Features.Inventory.Contracts;
using Agrivision.Backend.Domain.Abstractions;
using MediatR;

namespace Agrivision.Backend.Application.Features.Inventory.Queries;

public record GetAllInventoryItemTransactionsQuery
(
    Guid FarmId,
    string RequesterId
) : IRequest<Result<List<InventoryItemTransactionResponse>>>;