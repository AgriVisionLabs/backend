using Agrivision.Backend.Application.Features.Notifications.Commands;
using Agrivision.Backend.Application.Repositories.Core;
using Agrivision.Backend.Domain.Abstractions;
using Agrivision.Backend.Domain.Entities.Core;
using MediatR;

namespace Agrivision.Backend.Application.Features.Notifications.Handlers;

public class MarkAllNotificationsReadCommandHandler(INotificationRepository notificationRepository, IReadNotificationRepository readNotificationRepository) : IRequestHandler<MarkAllNotificationsReadCommand, Result>
{
    public async Task<Result> Handle(MarkAllNotificationsReadCommand request, CancellationToken cancellationToken)
    {
        var notifications = await notificationRepository
            .GetAllNotificationsByUserIdAsync(request.RequesterId, cancellationToken);

        if (notifications.Count == 0)
            return Result.Success();

        var alreadyReadIds = await readNotificationRepository
            .GetReadNotificationIdsByUserAsync(request.RequesterId, cancellationToken);

        var now = DateTime.UtcNow;

        var unreadNotifications = notifications
            .Where(n => !alreadyReadIds.Contains(n.Id))
            .Select(n => new ReadNotification
            {
                UserId = request.RequesterId,
                NotificationId = n.Id,
                ReadAt = now,
                CreatedById = request.RequesterId,
                CreatedOn = now
            })
            .ToList();

        if (unreadNotifications.Count > 0)
        {
            await readNotificationRepository.AddRangeAsync(unreadNotifications, cancellationToken);
        }

        return Result.Success();
    }
}