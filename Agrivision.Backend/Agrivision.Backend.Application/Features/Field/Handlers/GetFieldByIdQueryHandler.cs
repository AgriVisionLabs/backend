using Agrivision.Backend.Application.Errors;
using Agrivision.Backend.Application.Features.Farm.Contracts;
using Agrivision.Backend.Application.Features.Field.Contracts;
using Agrivision.Backend.Application.Features.Field.Queries;
using Agrivision.Backend.Application.Repositories.Core;
using Agrivision.Backend.Domain.Abstractions;
using MediatR;

namespace Agrivision.Backend.Application.Features.Field.Handlers;

public class GetFieldByIdQueryHandler(IFieldRepository fieldRepository, IFarmUserRoleRepository farmUserRoleRepository) : IRequestHandler<GetFieldByIdQuery, Result<FieldResponse>>
{
    public async Task<Result<FieldResponse>> Handle(GetFieldByIdQuery request, CancellationToken cancellationToken)
    {
        // check if field exists
        var field = await fieldRepository.FindByIdAsync(request.FieldId, cancellationToken);
        if (field is null)
            return Result.Failure<FieldResponse>(FieldErrors.FieldNotFound);
        
        // check if user has access to the farm
        var hasAccess = await farmUserRoleRepository.ExistsAsync(request.FarmId, request.RequesterId, cancellationToken);
        if (!hasAccess) 
            return Result.Failure<FieldResponse>(FarmErrors.UnauthorizedAction);
        
        // map to a response
        var response = new FieldResponse(field.Id, field.Name, field.Area, field.IsActive, field.FarmId);

        return Result.Success(response);
    }
}