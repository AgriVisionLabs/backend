using Agrivision.Backend.Application.Errors;
using Agrivision.Backend.Application.Features.Inventory.Commands;
using Agrivision.Backend.Application.Features.Inventory.Contracts;
using Agrivision.Backend.Application.Repositories.Core;
using Agrivision.Backend.Domain.Abstractions;
using Agrivision.Backend.Domain.Entities.Core;
using Agrivision.Backend.Domain.Enums.Core;
using MediatR;

namespace Agrivision.Backend.Application.Features.Inventory.Handlers;

public class AddInventoryItemCommandHandler(IFarmRepository farmRepository, IFieldRepository fieldRepository, IInventoryItemRepository inventoryItemRepository, IInventoryItemTransactionRepository inventoryItemTransactionRepository, IFarmUserRoleRepository farmUserRoleRepository) : IRequestHandler<AddInventoryItemCommand, Result<InventoryItemResponse>>
{
    public async Task<Result<InventoryItemResponse>> Handle(AddInventoryItemCommand request, CancellationToken cancellationToken)
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
            return Result.Failure<InventoryItemResponse>(FarmUserRoleErrors.InsufficientPermissions);
        
        // verify if the field is not null and if it exits
        Field? field = null;
        if (request.FieldId.HasValue)
        {
            field = await fieldRepository.FindByIdAsync(request.FieldId.Value, cancellationToken);
            if (field is null)
                return Result.Failure<InventoryItemResponse>(FieldErrors.FieldNotFound);
            
            // check if the field belong to the farm 
            if (field.FarmId != request.FarmId)
                return Result.Failure<InventoryItemResponse>(FarmErrors.UnauthorizedAction);
        }
        
        // check if name is used 
        if (await inventoryItemRepository.ExistsByFarmIdAndItemName(request.FarmId, request.Name, cancellationToken))
            return Result.Failure<InventoryItemResponse>(InventoryItemErrors.DuplicateName);
        
        // add item
        var item = new InventoryItem
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Category = request.Category,
            Quantity = request.Quantity,
            ThresholdQuantity = request.ThresholdQuantity,
            UnitCost = request.UnitCost,
            MeasurementUnit = request.MeasurementUnit,
            ExpirationDate = request.ExpirationDate,
            FarmId = request.FarmId,
            FieldId = request.FieldId,
            CreatedById = request.RequesterId,
            CreatedOn = DateTime.UtcNow
        };

        await inventoryItemRepository.AddAsync(item, cancellationToken); 

        var itemTransaction = new InventoryItemTransaction
        {
            Id = Guid.NewGuid(),
            InventoryItemId = item.Id,
            QuantityChanged = item.Quantity,
            Reason = InventoryTransactionType.InitialStock,
            CreatedById = request.RequesterId,
            CreatedOn = DateTime.UtcNow
        };

        await inventoryItemTransactionRepository.AddAsync(itemTransaction, cancellationToken);
        
        // map to response
        var response = new InventoryItemResponse(item.Id, item.FarmId, item.FieldId, item.CreatedById, item.Name,
            item.Category, item.Quantity, item.ThresholdQuantity, item.UnitCost, item.MeasurementUnit,
            item.ExpirationDate, item.DaysUntilExpiry, item.StockLevel);
        
        return Result.Success(response);
    }
}