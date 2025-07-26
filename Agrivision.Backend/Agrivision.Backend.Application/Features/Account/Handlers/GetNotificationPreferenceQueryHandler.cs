using Agrivision.Backend.Application.Features.Account.Contracts;
using Agrivision.Backend.Application.Features.Account.Queries;
using Agrivision.Backend.Application.Repositories.Core;
using Agrivision.Backend.Domain.Abstractions;
using Agrivision.Backend.Domain.Enums.Core;
using MediatR;

namespace Agrivision.Backend.Application.Features.Account.Handlers;

public class GetNotificationPreferenceQueryHandler(INotificationPreferenceRepository notificationPreferenceRepository) : IRequestHandler<GetNotificationPreferenceQuery, Result<NotificationPreferenceResponse>>
{
    public async Task<Result<NotificationPreferenceResponse>> Handle(GetNotificationPreferenceQuery request, CancellationToken cancellationToken)
    {
        var preferences = await notificationPreferenceRepository
            .GetByUserIdAsync(request.RequesterId, cancellationToken);

        if (preferences.Count == 0)
        {
            // return all false if user has no preferences
            return Result.Success(new NotificationPreferenceResponse(
                IsEnabled: false,
                Irrigation: false,
                Task: false,
                Message: false,
                Alert: false,
                Warning: false,
                SystemUpdate: false
            ));
        }

        // map preferences by type for fast access
        var preferenceMap = preferences.ToDictionary(p => p.NotificationType);

        bool GetFlag(NotificationType type)
            => preferenceMap.TryGetValue(type, out var pref) && pref.IsEnabled;

        var response = new NotificationPreferenceResponse(
            IsEnabled: true,
            Irrigation: GetFlag(NotificationType.Irrigation),
            Task: GetFlag(NotificationType.Task),
            Message: GetFlag(NotificationType.Message),
            Alert: GetFlag(NotificationType.Alert),
            Warning: GetFlag(NotificationType.Warning),
            SystemUpdate: GetFlag(NotificationType.SystemUpdate)
        );

        return Result.Success(response);
    }
}