using Agrivision.Backend.Application.Features.Account.Commands;
using Agrivision.Backend.Application.Repositories.Core;
using Agrivision.Backend.Domain.Abstractions;
using Agrivision.Backend.Domain.Entities.Core;
using Agrivision.Backend.Domain.Enums.Core;
using MediatR;

namespace Agrivision.Backend.Application.Features.Account.Handlers;

public class UpdateNotificationPreferenceCommandHandler(INotificationPreferenceRepository notificationPreferenceRepository) : IRequestHandler<UpdateNotificationPreferenceCommand, Result>
{
    private static readonly NotificationType[] NotificationTypes = [
        NotificationType.Irrigation,
        NotificationType.Task,
        NotificationType.Message,
        NotificationType.Alert,
        NotificationType.Warning,
        NotificationType.SystemUpdate
    ];

    public async Task<Result> Handle(UpdateNotificationPreferenceCommand request, CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;
        var userId = request.RequesterId;

        // Map enum values to request flags
        var typeFlags = new Dictionary<NotificationType, bool>
        {
            [NotificationType.Irrigation] = request.Irrigation,
            [NotificationType.Task] = request.Task,
            [NotificationType.Message] = request.Message,
            [NotificationType.Alert] = request.Alert,
            [NotificationType.Warning] = request.Warning,
            [NotificationType.SystemUpdate] = request.SystemUpdate
        };

        // Fetch existing preferences
        var existingPreferences = await notificationPreferenceRepository
            .GetByUserIdAsync(userId, cancellationToken);

        var preferencesMap = existingPreferences.ToDictionary(p => p.NotificationType);

        if (request.IsEnabled)
        {
            foreach (var type in NotificationTypes)
            {
                var isEnabled = typeFlags[type];
                if (preferencesMap.TryGetValue(type, out var preference))
                {
                    // Update existing preference
                    preference.IsEnabled = isEnabled;
                    preference.UpdatedById = userId;
                    preference.UpdatedOn = now;

                    await notificationPreferenceRepository.UpdateAsync(preference, cancellationToken);
                }
                else
                {
                    // Add new preference with audit fields
                    var newPreference = new NotificationPreference
                    {
                        UserId = userId,
                        NotificationType = type,
                        IsEnabled = isEnabled,
                        CreatedById = userId,
                        CreatedOn = now,
                        UpdatedById = userId,
                        UpdatedOn = now
                    };

                    await notificationPreferenceRepository.AddAsync(newPreference, cancellationToken);
                }
            }
        }
        else
        {
            // Disable all if they exist
            foreach (var preference in existingPreferences)
            {
                preference.IsEnabled = false;
                preference.UpdatedById = userId;
                preference.UpdatedOn = now;

                await notificationPreferenceRepository.UpdateAsync(preference, cancellationToken);
            }
        }
        
        return Result.Success();
    }
}