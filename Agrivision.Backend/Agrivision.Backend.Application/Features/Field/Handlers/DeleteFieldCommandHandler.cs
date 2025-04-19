using Agrivision.Backend.Application.Errors;
using Agrivision.Backend.Application.Features.Field.Commands;
using Agrivision.Backend.Application.Repositories.Core;
using Agrivision.Backend.Domain.Abstractions;
using MediatR;

namespace Agrivision.Backend.Application.Features.Field.Handlers;

public class DeleteFieldCommandHandler(IFieldRepository fieldRepository) : IRequestHandler<DeleteFieldCommand, Result>
{
    public async Task<Result> Handle(DeleteFieldCommand request, CancellationToken cancellationToken)
    {
        // check if the field exists
        var field = await fieldRepository.FindByIdWithFarmAsync(request.FieldId, cancellationToken);
        if (field is null)
            return Result.Failure(FieldErrors.FieldNotFound);
        
        // check if field belongs to provided farm
        if (field.FarmId != request.FarmId)
            return Result.Failure(FarmErrors.UnauthorizedAction);

        // check if user can delete
        if (request.DeletedById != field.Farm.CreatedById)
            return Result.Failure(FieldErrors.UnauthorizedAction);

        // soft delete
        field.IsDeleted = true;
        field.DeletedOn = DateTime.UtcNow;
        field.DeletedById = request.DeletedById;
        
        // update database
        await fieldRepository.UpdateAsync(field, cancellationToken);

        return Result.Success();
    }
}