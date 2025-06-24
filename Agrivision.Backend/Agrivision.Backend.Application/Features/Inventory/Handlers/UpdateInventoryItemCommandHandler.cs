using Agrivision.Backend.Application.Errors;
using Agrivision.Backend.Application.Features.Inventory.Commands;
using Agrivision.Backend.Application.Repositories.Core;
using Agrivision.Backend.Domain.Abstractions;
using Agrivision.Backend.Domain.Entities.Core;
using Agrivision.Backend.Domain.Enums.Core;
using MediatR;

namespace Agrivision.Backend.Application.Features.Inventory.Handlers;

public class UpdateInventoryItemCommandHandler(IFarmRepository farmRepository, IFieldRepository fieldRepository, IInventoryItemRepository inventoryItemRepository, IInventoryItemTransactionRepository inventoryItemTransactionRepository, IFarmUserRoleRepository farmUserRoleRepository) : IRequestHandler<UpdateInventoryItemCommand, Result>
{
    public async Task<Result> Handle(UpdateInventoryItemCommand request, CancellationToken cancellationToken)
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
        
        // expert can't update
        if (farmUserRole.FarmRole.Name == "Expert")
            return Result.Failure(FarmUserRoleErrors.InsufficientPermissions);
        
        // verify if the field is not null and if it exits
        Field? field = null;
        if (request.FieldId.HasValue)
        {
            field = await fieldRepository.FindByIdAsync(request.FieldId.Value, cancellationToken);
            if (field is null)
                return Result.Failure(FieldErrors.FieldNotFound);
            
            // check if the field belong to the farm 
            if (field.FarmId != request.FarmId)
                return Result.Failure(FarmErrors.UnauthorizedAction);
        }
        
        // get the item
        var item = await inventoryItemRepository.FindByIdAsync(request.ItemId, cancellationToken);
        if (item is null)
            return Result.Failure(InventoryItemErrors.ItemNotFound);
        
        // check if name is used 
        var existingItem =
            await inventoryItemRepository.FindByFarmIdAndItemNameAsync(request.FarmId, request.Name, cancellationToken);
        if (existingItem is not null && existingItem.Id != request.ItemId)
            return Result.Failure(InventoryItemErrors.DuplicateName);
        
        // create a transaction if quantity changed 
        if (item.Quantity != request.Quantity)
        {
            var transaction = new InventoryItemTransaction
            {
                Id = Guid.NewGuid(),
                InventoryItemId = item.Id,
                QuantityChanged = request.Quantity - item.Quantity,
                Reason = InventoryTransactionType.ManualAdjustment,
                CreatedById = request.RequesterId,
                CreatedOn = DateTime.UtcNow
            };

            await inventoryItemTransactionRepository.AddAsync(transaction, cancellationToken);
        }
        
        // update
        item.Name = request.Name;
        item.Category = request.Category;
        item.Quantity = request.Quantity;
        item.ThresholdQuantity = request.ThresholdQuantity;
        item.UnitCost = request.UnitCost;
        item.MeasurementUnit = request.MeasurementUnit;
        item.ExpirationDate = request.ExpirationDate;
        item.FieldId = request.FieldId;
        item.UpdatedById = request.RequesterId;
        item.UpdatedOn = DateTime.UtcNow;

        await inventoryItemRepository.UpdateAsync(item, cancellationToken);
        
        return Result.Success();
    }
}