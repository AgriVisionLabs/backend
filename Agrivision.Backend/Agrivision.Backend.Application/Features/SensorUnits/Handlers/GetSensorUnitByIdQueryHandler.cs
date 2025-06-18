using Agrivision.Backend.Application.Errors;
using Agrivision.Backend.Application.Features.SensorUnits.Contracts;
using Agrivision.Backend.Application.Features.SensorUnits.Queries;
using Agrivision.Backend.Application.Repositories.Core;
using Agrivision.Backend.Domain.Abstractions;
using Agrivision.Backend.Domain.Enums.Core;
using MediatR;

namespace Agrivision.Backend.Application.Features.SensorUnits.Handlers;

public class GetSensorUnitByIdQueryHandler(ISensorUnitRepository sensorUnitRepository, IFieldRepository fieldRepository, IFarmUserRoleRepository farmUserRoleRepository, ISensorReadingRepository sensorReadingRepository) : IRequestHandler<GetSensorUnitByIdQuery, Result<SensorUnitResponse>>
{
    public async Task<Result<SensorUnitResponse>> Handle(GetSensorUnitByIdQuery request, CancellationToken cancellationToken)
    {
        // check if the field exists
        var field = await fieldRepository.FindByIdAsync(request.FieldId, cancellationToken);
        if (field is null)
            return Result.Failure<SensorUnitResponse>(FieldErrors.FieldNotFound);
        
        // check if the field belongs to the farm
        if (field.FarmId != request.FarmId)
            return Result.Failure<SensorUnitResponse>(FarmErrors.UnauthorizedAction);
        
        // check if the user has access to the farm
        var farmUserRole =
            await farmUserRoleRepository.GetByUserAndFarmAsync(request.FarmId, request.RequesterId, cancellationToken);
        if (farmUserRole is null)
            return Result.Failure<SensorUnitResponse>(FarmErrors.UnauthorizedAction);
        
        // expert can't access irrigation units
        if (farmUserRole.FarmRole.Name == "Expert")
            return Result.Failure<SensorUnitResponse>(FarmErrors.UnauthorizedAction);
        
        // check if the sensor unit exists
        var sensorUnit = await sensorUnitRepository.FindByIdAsync(request.SensorUnitId, cancellationToken);
        if (sensorUnit is null)
            return Result.Failure<SensorUnitResponse>(SensorUnitErrors.SensorUnitNotFound);
        
        // check if the sensor unit belongs to the field
        if (sensorUnit.FieldId != field.Id)
            return Result.Failure<SensorUnitResponse>(FieldErrors.UnauthorizedAction);
        
        var moisture = 0f;
        var temperature = 0f;
        var humidity = 0f;

        var latestSensorReadings = await sensorReadingRepository
            .GetLatestReadingsByUnitIdAsync(sensorUnit.DeviceId, cancellationToken);

        if (latestSensorReadings.Count > 0)
        {
            moisture = latestSensorReadings.FirstOrDefault(r => r.Type == SensorType.Moisture)?.Value ?? 0f;

            temperature = latestSensorReadings.FirstOrDefault(r => r.Type == SensorType.Temperature)?.Value ?? 0f;

            humidity = latestSensorReadings.FirstOrDefault(r => r.Type == SensorType.Humidity)?.Value ?? 0f;
        }
        
        // map to response 
        var response = new SensorUnitResponse
            (sensorUnit.Id, sensorUnit.FarmId, sensorUnit.FieldId, field.Name, sensorUnit.Name, sensorUnit.IsOnline, sensorUnit.InstallationDate, sensorUnit.Status, sensorUnit.LastMaintenance, sensorUnit.NextMaintenance, sensorUnit.IpAddress, sensorUnit.Device.MacAddress, sensorUnit.Device.SerialNumber, sensorUnit.Device.FirmwareVersion, sensorUnit.CreatedById, sensorUnit.CreatedBy, sensorUnit.CreatedOn, moisture, temperature, humidity, sensorUnit.BatteryLevel);

        return Result.Success(response);
    }
}