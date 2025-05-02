using Agrivision.Backend.Application.Errors;
using Agrivision.Backend.Application.Features.IrrigationUnits.Commands;
using Agrivision.Backend.Application.Features.IrrigationUnits.Contracts;
using Agrivision.Backend.Application.Repositories.Core;
using Agrivision.Backend.Domain.Abstractions;
using Agrivision.Backend.Domain.Entities.Core;
using Agrivision.Backend.Domain.Enums.Core;
using MediatR;

namespace Agrivision.Backend.Application.Features.IrrigationUnits.Handlers;

public class AddIrrigationUnitCommandHandler(IIrrigationUnitRepository irrigationUnitRepository, IFieldRepository fieldRepository, IFarmUserRoleRepository farmUserRoleRepository, IIrrigationUnitDeviceRepository irrigationUnitDeviceRepository) : IRequestHandler<AddIrrigationUnitCommand, Result<IrrigationUnitResponse>>
{
    public async Task<Result<IrrigationUnitResponse>> Handle(AddIrrigationUnitCommand request, CancellationToken cancellationToken)
    {
        // check if field exists
        var field = await fieldRepository.FindByIdAsync(request.FieldId, cancellationToken);
        if (field is null)
            return Result.Failure<IrrigationUnitResponse>(FieldErrors.FieldNotFound);
        
        // check if field is in the specified farm
        if (field.FarmId != request.FarmId)
            return Result.Failure<IrrigationUnitResponse>(FarmErrors.UnauthorizedAction);
        
        // check if user has permission to access the farm
        var farmUserRole =
            await farmUserRoleRepository.GetByUserAndFarmAsync(request.FarmId, request.RequesterId, cancellationToken);
        if (farmUserRole is null)
            return Result.Failure<IrrigationUnitResponse>(FieldErrors.UnauthorizedAction);
        
        // check if the user has permission to add (only owner and manager can add)
        if (farmUserRole.FarmRole.Name != "Owner" && farmUserRole.FarmRole.Name != "Manager")
            return Result.Failure<IrrigationUnitResponse>(FarmUserRoleErrors.InsufficientPermission);
        
        // verify the device exists
        var device =
            await irrigationUnitDeviceRepository.FindBySerialNumberAsync(request.SerialNumber, cancellationToken);
        if (device is null)
            return Result.Failure<IrrigationUnitResponse>(IrrigationUnitDeviceErrors.DeviceNotFound);
        
        // check if it is already assigned 
        if (device.IsAssigned)
            return Result.Failure<IrrigationUnitResponse>(IrrigationUnitDeviceErrors.AlreadyAssigned);
        
        // check if the field doesn't already have an irrigation unit
        if (await irrigationUnitRepository.ExistsByFieldIdAsync(request.FieldId, cancellationToken))
            return Result.Failure<IrrigationUnitResponse>(FieldErrors.FieldAlreadyHasIrrigationUnit);
        
        // check if the farm doesn't have a unit with the same name 
        if (await irrigationUnitRepository.ExistsByNameAndFarmIdAsync(request.Name, request.FarmId, cancellationToken))
            return Result.Failure<IrrigationUnitResponse>(IrrigationUnitErrors.DuplicateNameInFarm);
        
        // add the unit
        var unit = new IrrigationUnit
        {
            Id = Guid.NewGuid(),
            DeviceId = device.Id,
            FarmId = request.FarmId,
            FieldId = field.Id,
            Name = request.Name,
            InstallationDate = DateTime.UtcNow,
            Status = IrrigationUnitStatus.Idle,
            CreatedById = request.RequesterId,
            CreatedBy = request.RequesterName,
            CreatedOn = DateTime.UtcNow
        };
        
        await irrigationUnitRepository.AddAsync(unit, cancellationToken);

        // update device to assigned 
        device.IsAssigned = true;
        device.AssignedAt = DateTime.UtcNow;

        await irrigationUnitDeviceRepository.UpdateAsync(device, cancellationToken);

        // map to response
        var response = new IrrigationUnitResponse(unit.Id, unit.FarmId, unit.FieldId, field.Name, unit.Name,
            unit.InstallationDate, unit.Status, null, null, null, device.MacAddress, device.FirmwareVersion,
            request.RequesterId, request.RequesterName, TimeSpan.Zero, DateTime.UtcNow);

        return Result.Success(response);
    }
}