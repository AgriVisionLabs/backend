using Agrivision.Backend.Application.Errors;
using Agrivision.Backend.Application.Features.SensorUnits.Commands;
using Agrivision.Backend.Application.Features.SensorUnits.Contracts;
using Agrivision.Backend.Application.Repositories.Core;
using Agrivision.Backend.Domain.Abstractions;
using Agrivision.Backend.Domain.Entities.Core;
using MediatR;

namespace Agrivision.Backend.Application.Features.SensorUnits.Handlers;

public class AddSensorUnitCommandHandler(ISensorUnitRepository sensorUnitRepository, ISensorUnitDeviceRepository sensorUnitDeviceRepository, IFieldRepository fieldRepository, IFarmUserRoleRepository farmUserRoleRepository) : IRequestHandler<AddSensorUnitCommand, Result<SensorUnitResponse>>
{
    public async Task<Result<SensorUnitResponse>> Handle(AddSensorUnitCommand request, CancellationToken cancellationToken)
    {
        // check if the field exists 
        var field = await fieldRepository.FindByIdAsync(request.FieldId, cancellationToken);
        if (field is null)
            return Result.Failure<SensorUnitResponse>(FieldErrors.FieldNotFound);
        
        // check if field belongs to the farm
        if (field.FarmId != request.FarmId)
            return Result.Failure<SensorUnitResponse>(FarmErrors.UnauthorizedAction);
        
        // check if the user has access to the farm
        var farmUserRole =
            await farmUserRoleRepository.GetByUserAndFarmAsync(request.FarmId, request.RequesterId, cancellationToken);
        if (farmUserRole is null)
            return Result.Failure<SensorUnitResponse>(FarmErrors.UnauthorizedAction);
        
        // check if the user has permission to add (only owner and manager can add)
        if (farmUserRole.FarmRole.Name != "Owner" && farmUserRole.FarmRole.Name != "Manager")
            return Result.Failure<SensorUnitResponse>(FarmUserRoleErrors.InsufficientPermissions);
        
        // check if the device exists
        var device = await sensorUnitDeviceRepository.FindBySerialNumberAsync(request.SerialNumber, cancellationToken);
        if (device is null)
            return Result.Failure<SensorUnitResponse>(SensorUnitDeviceErrors.DeviceNotFound);
        
        // check if the device is already assigned to another field
        if (device.IsAssigned)
            return Result.Failure<SensorUnitResponse>(SensorUnitDeviceErrors.AlreadyAssigned);
        
        // check if the sensor unit already exists
        var existingSensorUnit = await sensorUnitRepository.FindByNameAndFarmIdAsync(request.Name, request.FarmId, cancellationToken);
        if (existingSensorUnit is not null)
            return Result.Failure<SensorUnitResponse>(SensorUnitErrors.DuplicateNameInFarm);
        
        // create the sensor unit
        var sensorUnit = new SensorUnit
        {
            Id = Guid.NewGuid(),
            DeviceId = device.Id,
            FarmId = request.FarmId,
            FieldId = field.Id,
            Name = request.Name,
            InstallationDate = DateTime.UtcNow,
            CreatedById = request.RequesterId,
            CreatedBy = request.RequesterName,
            CreatedOn = DateTime.UtcNow,
            BatteryLevel = 78
        };
        
        // add the sensor unit to the repository
        await sensorUnitRepository.AddAsync(sensorUnit, cancellationToken);
        
        // mark the device as assigned
        device.IsAssigned = true;
        device.AssignedAt = DateTime.UtcNow;
        
        // update the device in the repository
        await sensorUnitDeviceRepository.UpdateAsync(device, cancellationToken);
        
        // map to response
        var response = new SensorUnitResponse
        (sensorUnit.Id, sensorUnit.FarmId, sensorUnit.FieldId, field.Name, sensorUnit.Name, sensorUnit.InstallationDate, sensorUnit.Status, null, null, null, device.MacAddress, device.SerialNumber, device.FirmwareVersion, sensorUnit.CreatedById, sensorUnit.CreatedBy, sensorUnit.CreatedOn, 0, 0, 0, sensorUnit.BatteryLevel);

        return Result.Success(response);
    }
}