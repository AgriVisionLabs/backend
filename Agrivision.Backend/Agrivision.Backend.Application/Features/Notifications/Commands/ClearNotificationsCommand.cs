using Agrivision.Backend.Domain.Abstractions;
using MediatR;

namespace Agrivision.Backend.Application.Features.Notifications.Commands;

public record ClearNotificationsCommand(string RequesterId) : IRequest<Result>;