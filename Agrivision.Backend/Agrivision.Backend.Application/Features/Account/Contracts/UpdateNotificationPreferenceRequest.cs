namespace Agrivision.Backend.Application.Features.Account.Contracts;

public record UpdateNotificationPreferenceRequest
(
    bool IsEnabled,
    bool Irrigation,
    bool Task,
    bool Message,
    bool Alert,
    bool Warning,
    bool SystemUpdate
);