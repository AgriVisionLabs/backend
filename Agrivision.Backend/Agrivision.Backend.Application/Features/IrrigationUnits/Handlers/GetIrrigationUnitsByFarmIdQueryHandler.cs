using Agrivision.Backend.Application.Errors;
using Agrivision.Backend.Application.Features.IrrigationUnits.Contracts;
using Agrivision.Backend.Application.Features.IrrigationUnits.Queries;
using Agrivision.Backend.Application.Repositories.Core;
using Agrivision.Backend.Domain.Abstractions;
using MediatR;

namespace Agrivision.Backend.Application.Features.IrrigationUnits.Handlers;

public class GetIrrigationUnitsByFarmIdQueryHandler(IFarmRepository farmRepository, IFarmUserRoleRepository farmUserRoleRepository, IIrrigationUnitRepository irrigationUnitRepository) : IRequestHandler<GetIrrigationUnitsByFarmIdQuery, Result<IReadOnlyList<IrrigationUnitResponse>>>
{
    public async Task<Result<IReadOnlyList<IrrigationUnitResponse>>> Handle(GetIrrigationUnitsByFarmIdQuery request, CancellationToken cancellationToken)
    {
        // check if the farm exists
        var farm = await farmRepository.FindByIdAsync(request.FarmId, cancellationToken);
        if (farm is null)
            return Result.Failure<IReadOnlyList<IrrigationUnitResponse>>(FarmErrors.FarmNotFound);
        
        // check if the user has access to the farm 
        var farmUserRole =
            await farmUserRoleRepository.FindByUserIdAndFarmIdAsync(request.RequesterId, request.FarmId, cancellationToken);
        if (farmUserRole is null)
            return Result.Failure<IReadOnlyList<IrrigationUnitResponse>>(FarmErrors.UnauthorizedAction);
        
        // expert can't access irrigation units
        if (farmUserRole.FarmRole.Name == "Expert")
            return Result.Failure<IReadOnlyList<IrrigationUnitResponse>>(FarmErrors.UnauthorizedAction);
        
        // get the irrigation unit
        var units = await irrigationUnitRepository.FindByFarmIdAsync(farm.Id, cancellationToken);
        
        // map to response
        var responses = new List<IrrigationUnitResponse>();

        foreach (var unit in units)
        {
            var duration = TimeSpan.Zero;

            if (unit.LastActivation.HasValue && unit.LastDeactivation.HasValue)
            {
                duration = unit.LastDeactivation.Value - unit.LastActivation.Value;
            }

            var response = new IrrigationUnitResponse(
                unit.Id,
                unit.FarmId,
                unit.FieldId,
                unit.Field.Name,
                unit.Name,
                unit.IsOnline,
                unit.IsOn,
                unit.InstallationDate,
                unit.Status,
                unit.LastMaintenance,
                unit.NextMaintenance,
                unit.IpAddress,
                unit.Device.MacAddress,
                unit.Device.SerialNumber,
                unit.Device.FirmwareVersion,
                unit.CreatedById,
                unit.CreatedBy,
                duration,
                unit.UpdatedOn ?? unit.CreatedOn
            );

            responses.Add(response);
        }

        return Result.Success<IReadOnlyList<IrrigationUnitResponse>>(responses);
    }
}