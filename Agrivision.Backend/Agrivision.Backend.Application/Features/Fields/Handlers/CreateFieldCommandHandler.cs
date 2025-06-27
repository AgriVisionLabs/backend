using Agrivision.Backend.Application.Errors;
using Agrivision.Backend.Application.Features.Fields.Commands;
using Agrivision.Backend.Application.Features.Fields.Contracts;
using Agrivision.Backend.Application.Repositories.Core;
using Agrivision.Backend.Domain.Abstractions;
using MediatR;

namespace Agrivision.Backend.Application.Features.Fields.Handlers;

public class CreateFieldCommandHandler (IFieldRepository fieldRepository, IFarmRepository farmRepository) : IRequestHandler<CreateFieldCommand, Result<FieldResponse>>
{
    public async Task<Result<FieldResponse>> Handle(CreateFieldCommand request, CancellationToken cancellationToken)
    {
        // check if the farm exists
        var farm = await farmRepository.FindByIdWithFieldsAsync(request.FarmId, cancellationToken);
        if (farm is null)
            return Result.Failure<FieldResponse>(FarmErrors.FarmNotFound);
        
        // check if farm owner
        if (farm.CreatedById != request.CreatedById)
            return Result.Failure<FieldResponse>(FieldErrors.UnauthorizedAction);
        
        // check if field name is not used 
        var existingField =
            await fieldRepository.FindByNameAndFarmIdAsync(request.Name, request.FarmId, cancellationToken);
        if (existingField is not null)
            return Result.Failure<FieldResponse>(FieldErrors.DuplicateFieldName);
        
        // calculate used area
        var usedArea = farm.Fields
            .Where(f => !f.IsDeleted)
            .Sum(f => f.Area); 
        
        // check if field area is appropriate
        if (usedArea + request.Area > farm.Area)
            return Result.Failure<FieldResponse>(FieldErrors.InvalidFieldArea);


        // map to field
        var field = new Domain.Entities.Core.Field
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Area = request.Area,
            IsActive = false,
            FarmId = farm.Id,
            CreatedOn = DateTime.UtcNow,
            CreatedById = request.CreatedById,
            IsDeleted = false
        };

        farm.FieldsNo++;

        await farmRepository.UpdateAsync(farm, cancellationToken);

        await fieldRepository.AddAsync(field, cancellationToken);

        return Result.Success(new FieldResponse(field.Id, field.Name, field.Area, field.IsActive, field.FarmId));
    }
}