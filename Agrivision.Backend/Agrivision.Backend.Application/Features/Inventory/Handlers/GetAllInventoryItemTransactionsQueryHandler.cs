using Agrivision.Backend.Application.Errors;
using Agrivision.Backend.Application.Features.Inventory.Contracts;
using Agrivision.Backend.Application.Features.Inventory.Queries;
using Agrivision.Backend.Application.Repositories.Core;
using Agrivision.Backend.Domain.Abstractions;
using MediatR;

namespace Agrivision.Backend.Application.Features.Inventory.Handlers;

public class GetAllInventoryItemTransactionsQueryHandler(IInventoryItemTransactionRepository inventoryItemTransactionRepository, IFarmUserRoleRepository farmUserRoleRepository) : IRequestHandler<GetAllInventoryItemTransactionsQuery, Result<List<InventoryItemTransactionResponse>>>
{
    public async Task<Result<List<InventoryItemTransactionResponse>>> Handle(GetAllInventoryItemTransactionsQuery request, CancellationToken cancellationToken)
    {
        // check if the requester has access to the farm
        var farmUserRole =
            await farmUserRoleRepository.FindByUserIdAndFarmIdAsync(request.RequesterId, request.FarmId,
                cancellationToken);
        if (farmUserRole is null || farmUserRole.FarmRole.Name == "Worker" || farmUserRole.FarmRole.Name == "Manager")
            return Result.Failure<List<InventoryItemTransactionResponse>>(FarmErrors.UnauthorizedAction);

        var transactions =
            await inventoryItemTransactionRepository.GetAllByFarmIdAsync(request.FarmId, cancellationToken);
        
        // map to response
        var response = transactions
            .Select(t => new InventoryItemTransactionResponse(
                t.Id,
                t.InventoryItem.FarmId,
                t.InventoryItem.Name,
                t.InventoryItem.Category,
                t.InventoryItem.UnitCost,
                t.InventoryItem.MeasurementUnit,
                t.CreatedOn,
                t.QuantityChanged,
                t.Reason
            ))
            .ToList();

        return Result.Success(response);
    }
}