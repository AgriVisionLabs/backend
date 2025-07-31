using Agrivision.Backend.Domain.Abstractions;
using MediatR;

namespace Agrivision.Backend.Application.Features.Notifications.Commands;

public record MarkAllNotificationsReadCommand(
    string RequesterId
) : IRequest<Result>;
