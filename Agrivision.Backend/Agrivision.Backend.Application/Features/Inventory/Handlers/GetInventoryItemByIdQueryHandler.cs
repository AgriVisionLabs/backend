using Agrivision.Backend.Application.Errors;
using Agrivision.Backend.Application.Features.Inventory.Contracts;
using Agrivision.Backend.Application.Features.Inventory.Queries;
using Agrivision.Backend.Application.Repositories.Core;
using Agrivision.Backend.Domain.Abstractions;
using MediatR;

namespace Agrivision.Backend.Application.Features.Inventory.Handlers;

public class GetInventoryItemByIdQueryHandler(IFarmRepository farmRepository, IInventoryItemRepository inventoryItemRepository, IFarmUserRoleRepository farmUserRoleRepository) : IRequestHandler<GetInventoryItemByIdQuery, Result<InventoryItemResponse>>
{
    public async Task<Result<InventoryItemResponse>> Handle(GetInventoryItemByIdQuery request, CancellationToken cancellationToken)
    {
        // verify the farm exists
        var farm = await farmRepository.FindByIdAsync(request.FarmId, cancellationToken);
        if (farm is null)
            return Result.Failure<InventoryItemResponse>(FarmErrors.FarmNotFound);
        
        // verify the user has access to the farm 
        var farmUserRole =
            await farmUserRoleRepository.FindByUserIdAndFarmIdAsync(request.RequesterId, request.FarmId,
                cancellationToken);
        if (farmUserRole is null)
            return Result.Failure<InventoryItemResponse>(FarmErrors.UnauthorizedAction);
        
        // expert can't add
        if (farmUserRole.FarmRole.Name == "Expert")
            return Result.Failure<InventoryItemResponse>(FarmErrors.UnauthorizedAction);
        
        // get items
        var item = await inventoryItemRepository.GetByIdAsync(request.ItemId, cancellationToken);
        if (item is null)
            return Result.Failure<InventoryItemResponse>(InventoryItemErrors.ItemNotFound);
        
        // verify it belongs to the specified farm
        if (item.FarmId != request.FarmId)
            return Result.Failure<InventoryItemResponse>(InventoryItemErrors.ItemNotFound);
        
        var response = new InventoryItemResponse(item.Id, item.FarmId, item.FieldId, item.CreatedById, item.Name,
            item.Category, item.Quantity, item.ThresholdQuantity, item.UnitCost, item.MeasurementUnit,
            item.ExpirationDate, item.DaysUntilExpiry, item.StockLevel);

        return Result.Success(response);
    }
}