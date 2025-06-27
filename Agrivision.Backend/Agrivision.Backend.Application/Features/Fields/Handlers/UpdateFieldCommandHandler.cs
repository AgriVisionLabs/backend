using Agrivision.Backend.Application.Errors;
using Agrivision.Backend.Application.Features.Fields.Commands;
using Agrivision.Backend.Application.Repositories.Core;
using Agrivision.Backend.Domain.Abstractions;
using MediatR;

namespace Agrivision.Backend.Application.Features.Fields.Handlers;

public class UpdateFieldCommandHandler(IFieldRepository fieldRepository, IFarmRepository farmRepository) : IRequestHandler<UpdateFieldCommand, Result>
{
    public async Task<Result> Handle(UpdateFieldCommand request, CancellationToken cancellationToken)
    {
        // check if field exist
        var field = await fieldRepository.FindByIdAsync(request.FieldId, cancellationToken);
        if (field is null)
            return Result.Failure(FieldErrors.FieldNotFound);
        
        // confirm field belongs to provided farm
        if (field.FarmId != request.FarmId)
            return Result.Failure(FarmErrors.UnauthorizedAction);
        
        // fetch the farm
        var farm = await farmRepository.FindByIdWithFieldsAsync(field.FarmId, cancellationToken);
        if (farm is null)
            return Result.Failure(FarmErrors.FarmNotFound);
        
        // check for duplicate field name in the same farm
        var existingField =
            await fieldRepository.FindByNameAndFarmIdAsync(request.Name, farm.Id, cancellationToken);
        if (existingField is not null && existingField.Id != field.Id && !existingField.IsDeleted)
            return Result.Failure(FieldErrors.DuplicateFieldName);
        
        // only owner can update fields (and farms)
        if (farm.CreatedById != request.UpdatedById)
            return Result.Failure(FieldErrors.UnauthorizedAction);
        
        // calculate used area
        var usedArea = farm.Fields
            .Where(f => !f.IsDeleted && f.Id != field.Id) // to get the sum of all and not include the one we are updating 
            .Sum(f => f.Area);
        
        // check if field area is appropriate
        if (usedArea + request.Area > farm.Area)
            return Result.Failure(FieldErrors.InvalidFieldArea);


        // update
        field.Name = request.Name;
        field.Area = request.Area;
        field.UpdatedOn = DateTime.UtcNow;
        field.UpdatedById = request.UpdatedById;
        
        // save
        await fieldRepository.UpdateAsync(field, cancellationToken);

        return Result.Success();
    }
}