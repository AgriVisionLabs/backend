using Agrivision.Backend.Application.Features.Hubs.Commands;
using Agrivision.Backend.Application.Services.Hubs;
using MediatR;

namespace Agrivision.Backend.Application.Features.Hubs.Handlers;

public class UnsubscribeFromFarmCommandHandler(ISensorHubNotifier sensorHubNotifier, IFarmConnectionTracker farmConnectionTracker) : IRequestHandler<UnsubscribeFromFarmCommand, Unit>
{
    public async Task<Unit> Handle(UnsubscribeFromFarmCommand request, CancellationToken cancellationToken)
    {
        if (farmConnectionTracker.TryRemove(request.ConnectionId, out var farmId))
        {
            await sensorHubNotifier.RemoveConnectionFromFarmGroup(request.ConnectionId, farmId);
        }

        return Unit.Value;
    }
}