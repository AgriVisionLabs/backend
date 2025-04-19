using Agrivision.Backend.Application.Errors;
using Agrivision.Backend.Application.Features.Farm.Commands;
using Agrivision.Backend.Application.Repositories.Core;
using Agrivision.Backend.Domain.Abstractions;
using MediatR;

namespace Agrivision.Backend.Application.Features.Farm.Handlers;

public class DeleteFarmCommandHandler(IFarmRepository farmRepository, IFarmUserRoleRepository farmUserRoleRepository) : IRequestHandler<DeleteFarmCommand, Result>
{
    public async Task<Result> Handle(DeleteFarmCommand request, CancellationToken cancellationToken)
    {
        // check if the farm exists
        var farm = await farmRepository.FindByIdWithFieldsAndRolesAsync(request.Id, cancellationToken);
        if (farm is null)
            return Result.Failure(FarmErrors.FarmNotFound);
        
        // check if user can delete
        if (request.DeletedById != farm.CreatedById)
            return Result.Failure(FarmErrors.UnauthorizedAction);
        
        // soft delete the farm
        farm.IsDeleted = true;
        farm.DeletedById = request.DeletedById;
        farm.DeletedOn = DateTime.UtcNow;
        
        // soft delete the fields in the farm
        foreach (var field in farm.Fields.Where(f => !f.IsDeleted))
        {
            field.IsDeleted = true;
            field.DeletedOn = DateTime.UtcNow;
            field.DeletedById = request.DeletedById;
        }
        
        // soft delete the farm user roles
        foreach (var farmUserRole in farm.FarmUserRoles.Where(fur => !fur.IsDeleted))
        {
            farmUserRole.IsDeleted = true;
            farmUserRole.DeletedOn = DateTime.UtcNow;
            farmUserRole.DeletedById = request.DeletedById;
        }
        
        await farmRepository.UpdateAsync(farm, cancellationToken);
        
        return Result.Success();
    }
}