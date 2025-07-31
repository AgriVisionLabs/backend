using Agrivision.Backend.Application.Errors;
using Agrivision.Backend.Application.Features.Notifications.Commands;
using Agrivision.Backend.Application.Repositories.Core;
using Agrivision.Backend.Domain.Abstractions;
using Agrivision.Backend.Domain.Entities.Core;
using MediatR;

namespace Agrivision.Backend.Application.Features.Notifications.Handlers;

public class MarkNotificationReadCommandHandler(INotificationRepository notificationRepository, IReadNotificationRepository readNotificationRepository) : IRequestHandler<MarkNotificationReadCommand, Result>
{
    public async Task<Result> Handle(MarkNotificationReadCommand request, CancellationToken cancellationToken)
    {
        // check if notification exists
        var notification = await notificationRepository
            .GetByIdAsync(request.NotificationId, request.RequesterId, cancellationToken);
        if (notification == null)
            return Result.Failure(NotificationsErrors.NotificationNotFound);
        
        // check if already read
        var alreadyRead = await readNotificationRepository
            .IsNotificationReadAsync(request.NotificationId, request.RequesterId, cancellationToken);
        if (alreadyRead)
            return Result.Success();
        
        var readNotification = new ReadNotification
        {
            UserId = request.RequesterId,
            ReadAt = DateTime.UtcNow,
            NotificationId = request.NotificationId,
            CreatedById = request.RequesterId,
            CreatedOn = DateTime.UtcNow
        };

        await readNotificationRepository.AddAsync(readNotification, cancellationToken);
        
        return Result.Success();
    }
}