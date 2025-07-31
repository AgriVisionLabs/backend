using Agrivision.Backend.Application.Errors;
using Agrivision.Backend.Application.Features.Inventory.Contracts;
using Agrivision.Backend.Application.Features.Inventory.Queries;
using Agrivision.Backend.Application.Repositories.Core;
using Agrivision.Backend.Domain.Abstractions;
using MediatR;

namespace Agrivision.Backend.Application.Features.Inventory.Handlers;

public class GetAllInventoryItemsByFarmIdQueryHandler(IFarmRepository farmRepository, IInventoryItemRepository inventoryItemRepository, IFarmUserRoleRepository farmUserRoleRepository): IRequestHandler<GetAllInventoryItemsByFarmIdQuery, Result<IReadOnlyList<InventoryItemResponse>>>
{
    public async Task<Result<IReadOnlyList<InventoryItemResponse>>> Handle(GetAllInventoryItemsByFarmIdQuery request, CancellationToken cancellationToken)
    {
        // verify the farm exists
        var farm = await farmRepository.FindByIdAsync(request.FarmId, cancellationToken);
        if (farm is null)
            return Result.Failure<IReadOnlyList<InventoryItemResponse>>(FarmErrors.FarmNotFound);
        
        // verify the user has access to the farm 
        var farmUserRole =
            await farmUserRoleRepository.FindByUserIdAndFarmIdAsync(request.RequesterId, request.FarmId,
                cancellationToken);
        if (farmUserRole is null)
            return Result.Failure<IReadOnlyList<InventoryItemResponse>>(FarmErrors.UnauthorizedAction);
        
        // expert can't add
        if (farmUserRole.FarmRole.Name == "Expert")
            return Result.Failure<IReadOnlyList<InventoryItemResponse>>(FarmErrors.UnauthorizedAction);
        
        // get items
        var items = await inventoryItemRepository.GetAllByFarmIdAsync(request.FarmId, cancellationToken);
        
        // map to response
        var response = items
            .Select(item => new InventoryItemResponse(
                item.Id,
                item.FarmId,
                item.FieldId,
                item.Field?.Name,
                item.CreatedById,
                item.Name,
                item.Category,
                item.Quantity,
                item.ThresholdQuantity,
                item.UnitCost,
                item.MeasurementUnit,
                item.ExpirationDate,
                item.DaysUntilExpiry,
                item.StockLevel
            ))
            .ToList();

        return Result.Success<IReadOnlyList<InventoryItemResponse>>(response);
    }
}