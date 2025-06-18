using Agrivision.Backend.Application.Errors;
using Agrivision.Backend.Application.Features.SensorUnits.Contracts;
using Agrivision.Backend.Application.Features.SensorUnits.Queries;
using Agrivision.Backend.Application.Repositories.Core;
using Agrivision.Backend.Domain.Abstractions;
using Agrivision.Backend.Domain.Enums.Core;
using MediatR;

namespace Agrivision.Backend.Application.Features.SensorUnits.Handlers;

public class GetSensorUnitsByFarmIdQueryHandler(IFarmRepository farmRepository, IFarmUserRoleRepository farmUserRoleRepository, ISensorUnitRepository sensorUnitRepository, ISensorReadingRepository sensorReadingRepository) : IRequestHandler<GetSensorUnitsByFarmIdQuery, Result<IReadOnlyList<SensorUnitResponse>>>
{
    public async Task<Result<IReadOnlyList<SensorUnitResponse>>> Handle(GetSensorUnitsByFarmIdQuery request, CancellationToken cancellationToken)
    {
        // check if farm exists
        var farm = await farmRepository.FindByIdAsync(request.FarmId, cancellationToken);
        if (farm is null)
            return Result.Failure<IReadOnlyList<SensorUnitResponse>>(FarmErrors.FarmNotFound);
        
        // check if user has access to the farm 
        var farmUserRole = await farmUserRoleRepository.GetByUserAndFarmAsync(request.FarmId, request.RequesterId, cancellationToken);
        if (farmUserRole is null)
            return Result.Failure<IReadOnlyList<SensorUnitResponse>>(FarmErrors.UnauthorizedAction);
        
        // expert can't access irrigation units
        if (farmUserRole.FarmRole.Name == "Expert")
            return Result.Failure<IReadOnlyList<SensorUnitResponse>>(FarmErrors.UnauthorizedAction);
        
        var sensorUnits = await sensorUnitRepository.FindByFarmIdAsync(request.FarmId, cancellationToken);

        var responses = new List<SensorUnitResponse>();

        foreach (var su in sensorUnits)
        {
            float moisture = 0f, temperature = 0f, humidity = 0f;

            var readings = await sensorReadingRepository
                .GetLatestReadingsByUnitIdAsync(su.DeviceId, cancellationToken);

            if (readings.Count > 0)
            {
                moisture = readings.FirstOrDefault(r => r.Type == SensorType.Moisture)?.Value ?? 0f;
                temperature = readings.FirstOrDefault(r => r.Type == SensorType.Temperature)?.Value ?? 0f;
                humidity = readings.FirstOrDefault(r => r.Type == SensorType.Humidity)?.Value ?? 0f;
            }

            responses.Add(new SensorUnitResponse(
                su.Id,
                su.FarmId,
                su.FieldId,
                su.Field?.Name ?? "",
                su.Name,
                su.IsOnline,
                su.InstallationDate,
                su.Status,
                su.LastMaintenance,
                su.NextMaintenance,
                su.IpAddress,
                su.Device?.MacAddress ?? "",
                su.Device?.SerialNumber ?? "",
                su.Device?.FirmwareVersion ?? "",
                su.CreatedById,
                su.CreatedBy,
                su.CreatedOn,
                moisture,
                temperature,
                humidity,
                su.BatteryLevel
            ));
        }

        return Result.Success<IReadOnlyList<SensorUnitResponse>>(responses);
    }
}