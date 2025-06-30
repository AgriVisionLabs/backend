
using Agrivision.Backend.Domain.Abstractions;
using MediatR;

namespace Agrivision.Backend.Application.Features.Subscription.Commands;
public record ConfirmSubscriptionCommand
    (
    string SessionId
    ) : IRequest<Result>;