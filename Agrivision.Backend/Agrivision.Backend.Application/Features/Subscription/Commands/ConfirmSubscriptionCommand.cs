
using Agrivision.Backend.Domain.Abstractions;
using MediatR;

namespace Agrivision.Backend.Application.Features.Subscription.Commands;
public record ConfirmSubscriptionCommand
    (
    string CustomerEmail,
    string SubscriptionId,
    Guid? PlanId
    ) : IRequest<Result>;