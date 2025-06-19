using Agrivision.Backend.Application.Errors;
using Agrivision.Backend.Application.Features.Hubs.Commands;
using Agrivision.Backend.Application.Repositories.Core;
using Agrivision.Backend.Application.Services.Hubs;
using Agrivision.Backend.Domain.Abstractions;
using MediatR;

namespace Agrivision.Backend.Application.Features.Hubs.Handlers;

public class SubscribeToFarmCommandHandler(IFarmUserRoleRepository farmUserRoleRepository, ISensorHubNotifier sensorHubNotifier, IFarmConnectionTracker farmConnectionTracker) : IRequestHandler<SubscribeToFarmCommand, Result>
{
    public async Task<Result> Handle(SubscribeToFarmCommand request, CancellationToken cancellationToken)
    {
        var farmUserRole =
            await farmUserRoleRepository.FindByUserIdAndFarmIdAsync(request.UserId, request.FarmId, cancellationToken);
        if (farmUserRole is null)
            return Result.Failure(FarmErrors.UnauthorizedAction);
        
        if ( farmUserRole.FarmRole.Name == "Expert")
            return Result.Failure(FarmUserRoleErrors.InsufficientPermissions);

        farmConnectionTracker.Add(request.ConnectionId, request.FarmId);

        await sensorHubNotifier.AddConnectionToFarmGroup(request.ConnectionId, request.FarmId);

        return Result.Success();
    }
}