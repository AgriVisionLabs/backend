using Agrivision.Backend.Application.Errors;
using Agrivision.Backend.Application.Features.IrrigationUnits.Commands;
using Agrivision.Backend.Application.Repositories.Core;
using Agrivision.Backend.Domain.Abstractions;
using MediatR;

namespace Agrivision.Backend.Application.Features.IrrigationUnits.Handlers;

public class UpdateIrrigationUnitCommandHandler(IFieldRepository fieldRepository, IIrrigationUnitRepository irrigationUnitRepository, IFarmUserRoleRepository farmUserRoleRepository) : IRequestHandler<UpdateIrrigationUnitCommand, Result>
{
    public async Task<Result> Handle(UpdateIrrigationUnitCommand request, CancellationToken cancellationToken)
    {
        // check if field exists 
        var field = await fieldRepository.FindByIdAsync(request.FieldId, cancellationToken);
        if (field is null)
            return Result.Failure(FieldErrors.FieldNotFound);
        
        // check if field belongs to the farm id 
        if (field.FarmId != request.FarmId)
            return Result.Failure(FarmErrors.UnauthorizedAction);
        
        // check if user has access to the farm 
        var farmUserRole =
            await farmUserRoleRepository.FindByUserIdAndFarmIdAsync(request.RequesterId, request.FarmId, cancellationToken);
        if (farmUserRole is null)
            return Result.Failure(FarmErrors.UnauthorizedAction);
        
        // check if irrigation unit exists
        var unit = await irrigationUnitRepository.FindByFieldIdAsync(field.Id, cancellationToken);
        if (unit is null)
            return Result.Failure(IrrigationUnitErrors.NoUnitAssigned);
        
        // check if the user has permission to update (only expert can't)
        if (farmUserRole.FarmRole.Name == "Expert")
            return Result.Failure(FieldErrors.UnauthorizedAction);
        
        // worker can only update state
        if (farmUserRole.FarmRole.Name == "Worker" && (request.Name != unit.Name || request.NewFieldId != unit.FieldId))
            return Result.Failure(FieldErrors.UnauthorizedAction);
        
        // check if new field exists
        if (request.NewFieldId != unit.FieldId)
        {
            var newField = await fieldRepository.FindByIdAsync(request.NewFieldId, cancellationToken);
            if (newField is null || newField.FarmId != request.FarmId)
                return Result.Failure(FieldErrors.FieldNotFound);
            if (await irrigationUnitRepository.ExistsByFieldIdAsync(newField.Id, cancellationToken))
                return Result.Failure(FieldErrors.FieldAlreadyHasIrrigationUnit);

            unit.FieldId = newField.Id;
        }
        
        // check if the farm doesn't have a unit with the same name 
        var existing =
            await irrigationUnitRepository.FindByNameAndFarmIdAsync(request.Name, request.FarmId, cancellationToken);
        if (existing is not null && existing.FieldId != unit.FieldId)
            return Result.Failure(IrrigationUnitErrors.DuplicateNameInFarm);
        
        // update
        unit.Name = request.Name;
        unit.Status = request.Status;
        unit.UpdatedById = request.RequesterId;
        unit.UpdatedOn = DateTime.UtcNow;

        await irrigationUnitRepository.UpdateAsync(unit, cancellationToken);

        return Result.Success();
    }
}