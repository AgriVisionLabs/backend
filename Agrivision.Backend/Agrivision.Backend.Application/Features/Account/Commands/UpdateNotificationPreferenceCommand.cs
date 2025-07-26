using Agrivision.Backend.Domain.Abstractions;
using MediatR;

namespace Agrivision.Backend.Application.Features.Account.Commands;

public record UpdateNotificationPreferenceCommand(
    string RequesterId,
    bool IsEnabled,
    bool Irrigation,
    bool Task,
    bool Message,
    bool Alert,
    bool Warning,
    bool SystemUpdate
) : IRequest<Result>;