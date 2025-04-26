
using Agrivision.Backend.Domain.Abstractions;
using MediatR;

namespace Agrivision.Backend.Application.Features.Subscription.Commands;
public record ConfirmSubscriptionCommand
    (

    string UserId,
    Guid PlanId,
    string PaymentIntentId

    ) : IRequest<Result>;