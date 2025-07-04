using Agrivision.Backend.Application.Errors;
using Agrivision.Backend.Application.Features.Fields.Commands;
using Agrivision.Backend.Application.Repositories.Core;
using Agrivision.Backend.Domain.Abstractions;
using MediatR;

namespace Agrivision.Backend.Application.Features.Fields.Handlers;

public class DeleteFieldCommandHandler(IFieldRepository fieldRepository, IFarmRepository farmRepository, IPlantedCropRepository plantedCropRepository, IAutomationRuleRepository automationRuleRepository) : IRequestHandler<DeleteFieldCommand, Result>
{
    public async Task<Result> Handle(DeleteFieldCommand request, CancellationToken cancellationToken)
    {
        // check if the field exists
        var field = await fieldRepository.FindByIdWithAllAsync(request.FieldId, cancellationToken);
        if (field is null)
            return Result.Failure(FieldErrors.FieldNotFound);

        if (field.FarmId != request.FarmId)
            return Result.Failure(FarmErrors.UnauthorizedAction);

        if (request.DeletedById != field.Farm.CreatedById)
            return Result.Failure(FieldErrors.UnauthorizedAction);

        var now = DateTime.UtcNow;

        // soft delete the field
        field.IsDeleted = true;
        field.DeletedOn = now;
        field.DeletedById = request.DeletedById;

        // get all automation rules for this farm to check for field-related rules
        var farmAutomationRules = await automationRuleRepository.FindByFarmIdAsync(request.FarmId, cancellationToken);
        
        // delete automation rules that belong to this field (through irrigation unit or sensor unit)
        foreach (var rule in farmAutomationRules.Where(r => !r.IsDeleted))
        {
            // check if rule's irrigation unit belongs to this field
            bool shouldDeleteRule = rule.IrrigationUnit.FieldId == request.FieldId || rule.SensorUnit?.FieldId == request.FieldId;

            if (!shouldDeleteRule) continue;
            rule.IsDeleted = true;
            rule.DeletedOn = now;
            rule.DeletedById = request.DeletedById;
            
            await automationRuleRepository.UpdateAsync(rule, cancellationToken);
        }

        // update irrigation unit
        if (field.IrrigationUnit is { IsDeleted: false })
        {
            var iu = field.IrrigationUnit;
            iu.IsDeleted = true;
            iu.DeletedOn = now;
            iu.DeletedById = request.DeletedById;
            
            iu.Device.IsAssigned = false;
            iu.Device.AssignedAt = null;
            iu.Device.UpdatedById = request.DeletedById;
            iu.Device.UpdatedOn = now;

            // soft delete associated irrigation events
            foreach (var irrigationEvent in iu.IrrigationEvents.Where(ie => !ie.IsDeleted))
            {
                irrigationEvent.IsDeleted = true;
                irrigationEvent.DeletedOn = now;
                irrigationEvent.DeletedById = request.DeletedById;
            }
        }

        // update sensor units
        foreach (var su in field.SensorUnits.Where(su => !su.IsDeleted))
        {
            su.IsDeleted = true;
            su.DeletedOn = now;
            su.DeletedById = request.DeletedById;
            
            su.Device.IsAssigned = false;
            su.Device.AssignedAt = null;
            su.Device.UpdatedById = request.DeletedById;
            su.Device.UpdatedOn = now;
        }

        // soft delete tasks
        foreach (var task in field.TaskItems.Where(t => !t.IsDeleted))
        {
            task.IsDeleted = true;
            task.DeletedOn = now;
            task.DeletedById = request.DeletedById;
        }

        // soft delete planted crop and its disease detections
        if (field.PlantedCrop is { IsDeleted: false })
        {
            var plantedCrop = field.PlantedCrop;
            plantedCrop.IsDeleted = true;
            plantedCrop.DeletedOn = now;
            plantedCrop.DeletedById = request.DeletedById;

            // soft delete associated disease detections
            foreach (var diseaseDetection in plantedCrop.DiseaseDetections.Where(dd => !dd.IsDeleted))
            {
                diseaseDetection.IsDeleted = true;
                diseaseDetection.DeletedOn = now;
                diseaseDetection.DeletedById = request.DeletedById;
            }
        }

        // null field reference in inventory items
        foreach (var item in field.InventoryItems.Where(i => i.FieldId != null))
        {
            item.FieldId = null;
            item.UpdatedOn = now;
            item.UpdatedById = request.DeletedById;
            
            // soft delete inventory item transactions
            foreach (var transaction in item.Transactions.Where(t => !t.IsDeleted))
            {
                transaction.IsDeleted = true;
                transaction.DeletedOn = now;
                transaction.DeletedById = request.DeletedById;
            }
        }

        field.Farm.FieldsNo--;

        await farmRepository.UpdateAsync(field.Farm, cancellationToken);
        await fieldRepository.UpdateAsync(field, cancellationToken);

        return Result.Success();
    }
}