using Agrivision.Backend.Application.Errors;
using Agrivision.Backend.Application.Features.Farms.Commands;
using Agrivision.Backend.Application.Repositories.Core;
using Agrivision.Backend.Domain.Abstractions;
using MediatR;

namespace Agrivision.Backend.Application.Features.Farms.Handlers;

public class DeleteFarmCommandHandler(IFarmRepository farmRepository) : IRequestHandler<DeleteFarmCommand, Result>
{
    public async Task<Result> Handle(DeleteFarmCommand request, CancellationToken cancellationToken)
    {
        var farm = await farmRepository.FindByIdWithAllAsync(request.Id, cancellationToken);
        if (farm is null)
            return Result.Failure(FarmErrors.FarmNotFound);

        if (request.DeletedById != farm.CreatedById)
            return Result.Failure(FarmErrors.UnauthorizedAction);

        var now = DateTime.UtcNow;

        farm.IsDeleted = true;
        farm.DeletedOn = now;
        farm.DeletedById = request.DeletedById;

        foreach (var field in farm.Fields.Where(f => !f.IsDeleted))
        {
            field.IsDeleted = true;
            field.DeletedOn = now;
            field.DeletedById = request.DeletedById;

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

            foreach (var task in field.TaskItems.Where(t => !t.IsDeleted))
            {
                task.IsDeleted = true;
                task.DeletedOn = now;
                task.DeletedById = request.DeletedById;
            }
        }

        foreach (var invitation in farm.FarmInvitations.Where(i => !i.IsDeleted))
        {
            invitation.IsDeleted = true;
            invitation.DeletedOn = now;
            invitation.DeletedById = request.DeletedById;
        }

        foreach (var rule in farm.AutomationRules.Where(r => !r.IsDeleted))
        {
            rule.IsDeleted = true;
            rule.DeletedOn = now;
            rule.DeletedById = request.DeletedById;
        }

        foreach (var role in farm.FarmUserRoles.Where(fur => !fur.IsDeleted))
        {
            role.IsDeleted = true;
            role.DeletedOn = now;
            role.DeletedById = request.DeletedById;
        }
        
        foreach (var item in farm.InventoryItems.Where(i => !i.IsDeleted))
        {
            item.IsDeleted = true;
            item.DeletedOn = now;
            item.DeletedById = request.DeletedById;
        }
        
        await farmRepository.UpdateAsync(farm, cancellationToken);
        return Result.Success();
    }
}