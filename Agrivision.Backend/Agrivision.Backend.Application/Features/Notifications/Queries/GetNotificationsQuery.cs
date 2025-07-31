using Agrivision.Backend.Application.Features.Notifications.Contracts;
using Agrivision.Backend.Domain.Abstractions;
using MediatR;

namespace Agrivision.Backend.Application.Features.Notifications.Queries;

public record GetNotificationsQuery
(
    string RequesterId
) : IRequest<Result<IReadOnlyList<NotificationResponse>>>;  