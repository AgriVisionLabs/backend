using Agrivision.Backend.Domain.Abstractions;
using MediatR;

namespace Agrivision.Backend.Application.Features.Notifications.Commands;

public record MarkNotificationReadCommand
(
    Guid NotificationId,
    string RequesterId
) : IRequest<Result>;