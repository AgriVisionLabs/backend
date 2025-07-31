using Agrivision.Backend.Application.Features.Notifications.Commands;
using Agrivision.Backend.Application.Repositories.Core;
using Agrivision.Backend.Domain.Abstractions;
using Agrivision.Backend.Domain.Entities.Core;
using MediatR;

namespace Agrivision.Backend.Application.Features.Notifications.Handlers;

public class ClearNotificationsCommandHandler(IClearedNotificationRepository clearedNotificationRepository) : IRequestHandler<ClearNotificationsCommand, Result>
{
    public async Task<Result> Handle(ClearNotificationsCommand request, CancellationToken cancellationToken)
    {
        // Clear all notifications for the user
        var clearedNotification = new ClearedNotification
        {
            UserId = request.RequesterId,
            ClearedAt = DateTime.UtcNow,
            CreatedById = request.RequesterId,
            CreatedOn = DateTime.UtcNow
        };

        await clearedNotificationRepository.AddAsync(clearedNotification, cancellationToken);
        
        return Result.Success();
    }
}