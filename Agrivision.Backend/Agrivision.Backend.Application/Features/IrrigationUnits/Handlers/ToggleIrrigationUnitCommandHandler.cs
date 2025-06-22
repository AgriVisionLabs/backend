using Agrivision.Backend.Application.Errors;
using Agrivision.Backend.Application.Features.IrrigationUnits.Commands;
using Agrivision.Backend.Application.Features.IrrigationUnits.Contracts;
using Agrivision.Backend.Application.Repositories.Core;
using Agrivision.Backend.Application.Services.IoT;
using Agrivision.Backend.Domain.Abstractions;
using Agrivision.Backend.Domain.Entities.Core;
using MediatR;

namespace Agrivision.Backend.Application.Features.IrrigationUnits.Handlers;

public class ToggleIrrigationUnitCommandHandler(IFieldRepository fieldRepository, IIrrigationUnitRepository irrigationUnitRepository, IFarmUserRoleRepository farmUserRoleRepository, IWebSocketDeviceCommunicator communicator, IIrrigationEventRepository irrigationEventRepository) : IRequestHandler<ToggleIrrigationUnitCommand, Result<ToggleIrrigationUnitResponse>>
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
            await farmUserRoleRepository.FindByUserIdAndFarmIdAsync(request.RequesterId, request.FarmId, cancellationToken);
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
            return Result.Failure<ToggleIrrigationUnitResponse>(IrrigationUnitErrors.DeviceUnreachable);

        var now = DateTime.UtcNow;
        
        if (unit.IsOn)
        {
            unit.LastDeactivation = now;

            var openEvent = await irrigationEventRepository.FindLastestByIrrigationUnitIdAsync(unit.Id, cancellationToken);
            if (openEvent is not null && openEvent.EndTime is null)
            {
                openEvent.EndTime = now;
                await irrigationEventRepository.UpdateAsync(openEvent, cancellationToken);
            }
        }
        else
        {
            unit.LastActivation = now;

            var newEvent = new IrrigationEvent
            {
                Id = Guid.NewGuid(),
                CreatedById = request.RequesterId,
                CreatedOn = now,
                IrrigationUnitId = unit.Id,
                StartTime = now
            };

            await irrigationEventRepository.AddAsync(newEvent, cancellationToken);
        }

        unit.IsOn = !unit.IsOn;
        unit.ToggledById = request.RequesterId;
        unit.UpdatedOn = now;
        unit.UpdatedById = request.RequesterId;

        await irrigationUnitRepository.UpdateAsync(unit, cancellationToken);
        await communicator.SendConfirmationAsync(unit.DeviceId, "toggle_pump");

        var response = new ToggleIrrigationUnitResponse(unit.IsOn, unit.ToggledById, request.RequesterName);
        return Result.Success(response);
    }
}