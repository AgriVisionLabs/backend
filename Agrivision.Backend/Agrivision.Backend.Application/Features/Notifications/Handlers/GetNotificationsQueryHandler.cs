using Agrivision.Backend.Application.Features.Notifications.Contracts;
using Agrivision.Backend.Application.Features.Notifications.Queries;
using Agrivision.Backend.Application.Repositories.Core;
using Agrivision.Backend.Domain.Abstractions;
using MediatR;

namespace Agrivision.Backend.Application.Features.Notifications.Handlers;

public class GetNotificationsQueryHandler(INotificationRepository notificationRepository, IReadNotificationRepository readNotificationRepository) : IRequestHandler<GetNotificationsQuery, Result<IReadOnlyList<NotificationResponse>>>
{
    public async Task<Result<IReadOnlyList<NotificationResponse>>> Handle(GetNotificationsQuery request, CancellationToken cancellationToken)
    {
        var notifications = await notificationRepository
            .GetAllNotificationsByUserIdAsync(request.RequesterId, cancellationToken);

        if (notifications.Count == 0)
            return Result.Success<IReadOnlyList<NotificationResponse>>([]);

        var readNotificationIds = await readNotificationRepository
            .GetReadNotificationIdsByUserAsync(request.RequesterId, cancellationToken);

        var response = notifications.Select(n =>
            new NotificationResponse(
                Id: n.Id,
                Type: n.Type,
                Message: n.Message,
                FarmId: n.FarmId,
                FieldId: n.FieldId,
                CreatedOn: n.CreatedOn,
                IsRead: readNotificationIds.Contains(n.Id)
            )
        ).ToList();

        return Result.Success<IReadOnlyList<NotificationResponse>>(response);
    }
}