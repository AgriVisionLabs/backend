using Agrivision.Backend.Application.Errors;
using Agrivision.Backend.Application.Features.Fields.Contracts;
using Agrivision.Backend.Application.Features.Fields.Queries;
using Agrivision.Backend.Application.Repositories.Core;
using Agrivision.Backend.Domain.Abstractions;
using MediatR;

namespace Agrivision.Backend.Application.Features.Fields.Handlers;

public class GetAllFieldsByFarmIdQueryHandler(IFieldRepository fieldRepository, IFarmRepository farmRepository, IFarmUserRoleRepository farmUserRoleRepository) : IRequestHandler<GetAllFieldsByFarmIdQuery, Result<List<FieldResponse>>>
{
    public async Task<Result<List<FieldResponse>>> Handle(GetAllFieldsByFarmIdQuery request, CancellationToken cancellationToken)
    {
        // check if farm exists
        var farm = await farmRepository.FindByIdAsync(request.FarmId, cancellationToken);
        if (farm is null)
            return Result.Failure<List<FieldResponse>>(FarmErrors.FarmNotFound);
     
        // check if user has access to the farm
        var hasAccess = await farmUserRoleRepository.ExistsAsync(request.FarmId, request.RequesterId, cancellationToken);
        if (!hasAccess)
            return Result.Failure<List<FieldResponse>>(FarmErrors.UnauthorizedAction);
        
        // get all fields
        var fields = await fieldRepository.GetAllByFarmIdAsync(farm!.Id, cancellationToken);
        
        //map to response
        var response = fields.Select(field =>
            new FieldResponse(field.Id, field.Name, field.Area, field.IsActive, field.FarmId)).ToList();

        return Result.Success(response);
    }
}