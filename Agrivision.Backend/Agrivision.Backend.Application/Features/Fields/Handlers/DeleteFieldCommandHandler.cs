using Agrivision.Backend.Application.Errors;
using Agrivision.Backend.Application.Features.Fields.Commands;
using Agrivision.Backend.Application.Repositories.Core;
using Agrivision.Backend.Domain.Abstractions;
using MediatR;

namespace Agrivision.Backend.Application.Features.Fields.Handlers;

public class DeleteFieldCommandHandler(IFieldRepository fieldRepository, IFarmRepository farmRepository) : IRequestHandler<DeleteFieldCommand, Result>
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

        // null field reference in inventory items
        foreach (var item in field.InventoryItems.Where(i => i.FieldId != null))
        {
            item.FieldId = null;
            item.UpdatedOn = now;
            item.UpdatedById = request.DeletedById;
        }

        field.Farm.FieldsNo--;

        await farmRepository.UpdateAsync(field.Farm, cancellationToken);
        await fieldRepository.UpdateAsync(field, cancellationToken);

        return Result.Success();
    }
}