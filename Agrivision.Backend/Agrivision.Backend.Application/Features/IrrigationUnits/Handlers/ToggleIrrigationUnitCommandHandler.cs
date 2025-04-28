using Agrivision.Backend.Application.Errors;
using Agrivision.Backend.Application.Features.IrrigationUnits.Commands;
using Agrivision.Backend.Application.Repositories.Core;
using Agrivision.Backend.Application.Services.IoT;
using Agrivision.Backend.Domain.Abstractions;
using MediatR;

namespace Agrivision.Backend.Application.Features.IrrigationUnits.Handlers;

public class ToggleIrrigationUnitCommandHandler(IFieldRepository fieldRepository, IIrrigationUnitRepository irrigationUnitRepository, IFarmUserRoleRepository farmUserRoleRepository, IWebSocketDeviceCommunicator communicator) : IRequestHandler<ToggleIrrigationUnitCommand, Result>
{
    public async Task<Result> Handle(ToggleIrrigationUnitCommand request, CancellationToken cancellationToken)
    {
        // check if field exists
        var field = await fieldRepository.FindByIdAsync(request.FieldId, cancellationToken);
        if (field is null)
            return Result.Failure(FieldErrors.FieldNotFound);
        
        // check if field exists in specified farm
        if (field.FarmId != request.FarmId)
            return Result.Failure(FarmErrors.UnauthorizedAction);
        
        // check if irrigation unit exists (by field id)
        var unit = await irrigationUnitRepository.FindByFieldIdAsync(request.FieldId, cancellationToken);
        if (unit is null)
            return Result.Failure(IrrigationUnitErrors.UnitNotAssigned);
        
        // check if user has access to the farm
        var farmUserRole =
            await farmUserRoleRepository.GetByUserAndFarmAsync(request.FarmId, request.RequesterId, cancellationToken);
        if (farmUserRole is null)
            return Result.Failure(FarmErrors.UnauthorizedAction);
        if (farmUserRole.FarmRole.Name == "Expert")
            return Result.Failure(FieldErrors.UnauthorizedAction);
        
        // check if device is online
        if (!communicator.IsDeviceConnected(unit.DeviceId) || !unit.IsOnline)
            return Result.Failure(IrrigationUnitErrors.DeviceOffline);
        
        // send toggle command
        var success = await communicator.SendCommandAsync(unit.DeviceId, "toggle_pump");
        if (!success)
            return Result.Failure(IrrigationUnitErrors.FailedToSendCommand);

        var ack = communicator.GetLastAck(unit.DeviceId, "toggle_pump");
        if (ack is null || (DateTime.UtcNow - ack.Value).TotalSeconds > 5)
            return Result.Failure(IrrigationUnitErrors.DeviceUnreachable);

        return Result.Success();
    }
}