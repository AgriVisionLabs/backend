using Agrivision.Backend.Application.Features.Hubs.Commands;
using Agrivision.Backend.Application.Services.Hubs;
using Agrivision.Backend.Domain.Abstractions;
using MediatR;

namespace Agrivision.Backend.Application.Features.Hubs.Handlers;

public class SubscribeToNotificationUpdatesCommandHandler(INotificationHubNotifier notificationHubNotifier) : IRequestHandler<SubscribeToNotificationUpdatesCommand, Result>
{
    public async Task<Result> Handle(SubscribeToNotificationUpdatesCommand request, CancellationToken cancellationToken)
    {
        await notificationHubNotifier.AddConnectionAsync(request.ConnectionId, request.UserId);
        return Result.Success();
    }
} 