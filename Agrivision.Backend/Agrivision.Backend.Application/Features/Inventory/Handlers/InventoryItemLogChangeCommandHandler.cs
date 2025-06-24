using Agrivision.Backend.Application.Errors;
using Agrivision.Backend.Application.Features.Inventory.Commands;
using Agrivision.Backend.Application.Repositories.Core;
using Agrivision.Backend.Domain.Abstractions;
using Agrivision.Backend.Domain.Entities.Core;
using Agrivision.Backend.Domain.Enums.Core;
using MediatR;

namespace Agrivision.Backend.Application.Features.Inventory.Handlers;

public class InventoryItemLogChangeCommandHandler(IFarmRepository farmRepository, IInventoryItemRepository inventoryItemRepository, IInventoryItemTransactionRepository inventoryItemTransactionRepository, IFarmUserRoleRepository farmUserRoleRepository) : IRequestHandler<InventoryItemLogChangeCommand, Result>
{
    public async Task<Result> Handle(InventoryItemLogChangeCommand request, CancellationToken cancellationToken)
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
        
        // expert can't log changes
        if (farmUserRole.FarmRole.Name == "Expert")
            return Result.Failure(FarmUserRoleErrors.InsufficientPermissions);
        
        // get the item
        var item = await inventoryItemRepository.FindByIdAsync(request.ItemId, cancellationToken);
        if (item is null)
            return Result.Failure(InventoryItemErrors.ItemNotFound);
        
        // check if they have enough in the inventory
        if (request.Quantity < 0 && item.Quantity + request.Quantity < 0)
        {
            return Result.Failure(InventoryItemErrors.InsufficientInventoryQuantity);
        }
        
        // create a transaction 
        var transaction = new InventoryItemTransaction
        {
            Id = Guid.NewGuid(),
            InventoryItemId = item.Id,
            QuantityChanged = request.Quantity,
            Reason = request.Reason,
            CreatedById = request.RequesterId,
            CreatedOn = DateTime.UtcNow
        };

        await inventoryItemTransactionRepository.AddAsync(transaction, cancellationToken);
        
        // update the item 
        item.Quantity += request.Quantity;
        item.UpdatedById = request.RequesterId;
        item.UpdatedOn = DateTime.UtcNow;

        await inventoryItemRepository.UpdateAsync(item, cancellationToken);
        
        return Result.Success();
    }
}