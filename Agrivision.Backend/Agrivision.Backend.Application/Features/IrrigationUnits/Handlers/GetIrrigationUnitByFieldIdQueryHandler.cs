using Agrivision.Backend.Application.Errors;
using Agrivision.Backend.Application.Features.IrrigationUnits.Contracts;
using Agrivision.Backend.Application.Features.IrrigationUnits.Queries;
using Agrivision.Backend.Application.Repositories.Core;
using Agrivision.Backend.Domain.Abstractions;
using MediatR;

namespace Agrivision.Backend.Application.Features.IrrigationUnits.Handlers;

public class GetIrrigationUnitByFieldIdQueryHandler(IFieldRepository fieldRepository, IIrrigationUnitRepository irrigationUnitRepository, IFarmUserRoleRepository farmUserRoleRepository) : IRequestHandler<GetIrrigationUnitByFieldIdQuery, Result<IrrigationUnitResponse>>
{
    public async Task<Result<IrrigationUnitResponse>> Handle(GetIrrigationUnitByFieldIdQuery request, CancellationToken cancellationToken)
    {
        // check if field exists
        var field = await fieldRepository.FindByIdAsync(request.FieldId, cancellationToken);
        if (field is null)
            return Result.Failure<IrrigationUnitResponse>(FieldErrors.FieldNotFound);
        
        // check if field belongs to the farm
        if (field.FarmId != request.FarmId)
            return Result.Failure<IrrigationUnitResponse>(FarmErrors.UnauthorizedAction);
        
        // check if user has access to the farm 
        var farmUserRole =
            await farmUserRoleRepository.GetByUserAndFarmAsync(request.FarmId, request.RequesterId, cancellationToken);
        if (farmUserRole is null)
            return Result.Failure<IrrigationUnitResponse>(FarmErrors.UnauthorizedAction);
        
        // experts can't access irrigation units
        if (farmUserRole.FarmRole.Name == "Expert")
            return Result.Failure<IrrigationUnitResponse>(FieldErrors.UnauthorizedAction);
        
        // get the irrigation unit 
        var unit = await irrigationUnitRepository.FindByFieldIdAsync(request.FieldId, cancellationToken);
        if (unit is null)
            return Result.Failure<IrrigationUnitResponse>(IrrigationUnitErrors.NoUnitAssigned);
        
        TimeSpan duration = TimeSpan.Zero;

        if (unit.LastActivation.HasValue && unit.LastDeactivation.HasValue)
        {
            duration = unit.LastDeactivation.Value - unit.LastActivation.Value;
        }
        
        // map to response
        var response = new IrrigationUnitResponse(unit.Id, unit.FarmId, unit.FieldId, field.Name, unit.Name,
            unit.InstallationDate, unit.Status, unit.LastMaintenance, unit.NextMaintenance, unit.IpAddress,
            unit.Device.MacAddress, unit.Device.FirmwareVersion, unit.CreatedById, unit.CreatedBy, duration, unit.UpdatedOn ?? unit.CreatedOn);

        return Result.Success(response);
    }
}