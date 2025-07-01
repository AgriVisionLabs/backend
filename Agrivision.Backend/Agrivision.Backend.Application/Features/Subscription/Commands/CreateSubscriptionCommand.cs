using MediatR;
using Agrivision.Backend.Domain.Abstractions;
using Agrivision.Backend.Application.Features.Subscription.Contracts;
namespace Agrivision.Backend.Application.Features.Subscription.Commands;
public record CreateSubscriptionCommand
(
    string UserId,
    Guid PlanId
) : IRequest<Result<CreateSubscriptionResponse>>; // Return checkout session url
