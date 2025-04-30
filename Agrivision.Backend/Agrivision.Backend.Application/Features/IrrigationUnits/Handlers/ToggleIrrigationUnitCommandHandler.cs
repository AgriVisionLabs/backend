using Agrivision.Backend.Application.Errors;
using Agrivision.Backend.Application.Features.IrrigationUnits.Commands;
using Agrivision.Backend.Application.Features.IrrigationUnits.Contracts;
using Agrivision.Backend.Application.Repositories.Core;
using Agrivision.Backend.Application.Services.IoT;
using Agrivision.Backend.Domain.Abstractions;
using MediatR;

namespace Agrivision.Backend.Application.Features.IrrigationUnits.Handlers;

public class ToggleIrrigationUnitCommandHandler(IFieldRepository fieldRepository, IIrrigationUnitRepository irrigationUnitRepository, IFarmUserRoleRepository farmUserRoleRepository, IWebSocketDeviceCommunicator communicator) : IRequestHandler<ToggleIrrigationUnitCommand, Result<ToggleIrrigationUnitResponse>>
{
    public async Task<Result<ToggleIrrigationUnitResponse>> Handle(ToggleIrrigationUnitCommand request, CancellationToken cancellationToken)
    {
        // check if field exists
        var field = await fieldRepository.FindByIdAsync(request.FieldId, cancellationToken);
        if (field is null)
            return Result.Failure<ToggleIrrigationUnitResponse>(FieldErrors.FieldNotFound);
        
        // check if field exists in specified farm
        if (field.FarmId != request.FarmId)
            return Result.Failure<ToggleIrrigationUnitResponse>(FarmErrors.UnauthorizedAction);
        
        // check if irrigation unit exists (by field id)
        var unit = await irrigationUnitRepository.FindByFieldIdAsync(request.FieldId, cancellationToken);
        if (unit is null)
            return Result.Failure<ToggleIrrigationUnitResponse>(IrrigationUnitErrors.NoUnitAssigned);
        
        // check if user has access to the farm
        var farmUserRole =
            await farmUserRoleRepository.GetByUserAndFarmAsync(request.FarmId, request.RequesterId, cancellationToken);
        if (farmUserRole is null)
            return Result.Failure<ToggleIrrigationUnitResponse>(FarmErrors.UnauthorizedAction);
        if (farmUserRole.FarmRole.Name == "Expert")
            return Result.Failure<ToggleIrrigationUnitResponse>(FieldErrors.UnauthorizedAction);
        
        // check if device is online
        if (!communicator.IsDeviceConnected(unit.DeviceId) || !unit.IsOnline)
            return Result.Failure<ToggleIrrigationUnitResponse>(IrrigationUnitErrors.DeviceOffline);
        
        // send toggle command
        var ok = await communicator.SendCommandAsync(unit.DeviceId, "toggle_pump", cancellationToken);

        if (!ok)
            return Result.Failure<ToggleIrrigationUnitResponse>(
                IrrigationUnitErrors.DeviceUnreachable); 

        if (unit.IsOn)
            unit.LastDeactivation = DateTime.UtcNow;
        else
            unit.LastActivation = DateTime.UtcNow;
        
        unit.IsOn = !unit.IsOn;
        unit.ToggledById = request.RequesterId;
        unit.UpdatedOn = DateTime.UtcNow;
        unit.UpdatedById = request.RequesterId;

        await irrigationUnitRepository.UpdateAsync(unit, cancellationToken);

        await communicator.SendConfirmationAsync(unit.DeviceId, "toggle_pump");

        var response = new ToggleIrrigationUnitResponse(unit.IsOn, unit.ToggledById, request.RequesterName);

        return Result.Success(response);
    }
}