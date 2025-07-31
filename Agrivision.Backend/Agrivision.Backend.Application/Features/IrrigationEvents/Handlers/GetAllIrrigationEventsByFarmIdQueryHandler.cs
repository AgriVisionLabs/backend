using Agrivision.Backend.Application.Errors;
using Agrivision.Backend.Application.Features.IrrigationEvents.Contracts;
using Agrivision.Backend.Application.Features.IrrigationEvents.Queries;
using Agrivision.Backend.Application.Repositories.Core;
using Agrivision.Backend.Domain.Abstractions;
using MediatR;

namespace Agrivision.Backend.Application.Features.IrrigationEvents.Handlers;

public class GetAllIrrigationEventsByFarmIdQueryHandler(IIrrigationEventRepository irrigationEventRepository, IFarmUserRoleRepository farmUserRoleRepository) : IRequestHandler<GetAllIrrigationEventsByFarmIdQuery, Result<List<IrrigationEventResponse>>>
{
    public async Task<Result<List<IrrigationEventResponse>>> Handle(GetAllIrrigationEventsByFarmIdQuery request, CancellationToken cancellationToken)
    {
        // check if the requester has access to the farm
        var farmUserRole =
            await farmUserRoleRepository.FindByUserIdAndFarmIdAsync(request.RequesterId, request.FarmId,
                cancellationToken);
        if (farmUserRole is null || farmUserRole.FarmRole.Name == "Worker")
        {
            return Result.Failure<List<IrrigationEventResponse>>(FarmErrors.UnauthorizedAction);
        }
        
        var irrigationEvents = await irrigationEventRepository.GetAllByFarmIdAsync(request.FarmId, cancellationToken);

        var response = irrigationEvents
            .Select(ie => new IrrigationEventResponse(
                ie.Id,
                ie.IrrigationUnit.FarmId,
                ie.IrrigationUnit.Field.Id,
                ie.IrrigationUnit.Field.Name,
                ie.IrrigationUnit.Field.PlantedCrop?.Crop.Name ?? "No Crop",
                ie.IrrigationUnitId,
                ie.StartTime,
                ie.EndTime,
                ie.TriggerMethod
            ))
            .ToList();

        return Result.Success(response);
    }
}