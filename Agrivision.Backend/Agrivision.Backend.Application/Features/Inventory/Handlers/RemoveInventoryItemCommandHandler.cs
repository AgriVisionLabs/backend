using Agrivision.Backend.Application.Errors;
using Agrivision.Backend.Application.Features.Inventory.Commands;
using Agrivision.Backend.Application.Repositories.Core;
using Agrivision.Backend.Domain.Abstractions;
using MediatR;

namespace Agrivision.Backend.Application.Features.Inventory.Handlers;

public class RemoveInventoryItemCommandHandler(IFarmRepository farmRepository, IInventoryItemRepository inventoryItemRepository, IFarmUserRoleRepository farmUserRoleRepository) : IRequestHandler<RemoveInventoryItemCommand, Result>
{
    public async Task<Result> Handle(RemoveInventoryItemCommand request, CancellationToken cancellationToken)
    {
        // verify the farm exists
        var farm = await farmRepository.FindByIdAsync(request.FarmId, cancellationToken);
        if (farm is null)
            return Result.Failure(FarmErrors.FarmNotFound);
        
        // verify the user has access to the farm 
        var farmUserRole =
            await farmUserRoleRepository.FindByUserIdAndFarmIdAsync(request.RequesterId, request.FarmId,
                cancellationToken);
        if (farmUserRole is null)
            return Result.Failure(FarmErrors.UnauthorizedAction);
        
        // expert can't remove
        if (farmUserRole.FarmRole.Name == "Expert")
            return Result.Failure(FarmUserRoleErrors.InsufficientPermissions);
        
        // get the item
        var item = await inventoryItemRepository.FindByIdAsync(request.ItemId, cancellationToken);
        if (item is null)
            return Result.Failure(InventoryItemErrors.ItemNotFound);
        
        // mark as deleted
        item.IsDeleted = true;
        item.DeletedById = request.RequesterId;
        item.DeletedOn = DateTime.UtcNow;
        item.UpdatedById = request.RequesterId;
        item.UpdatedOn = DateTime.UtcNow;

        await inventoryItemRepository.UpdateAsync(item, cancellationToken);
        
        return Result.Success();
    }
}