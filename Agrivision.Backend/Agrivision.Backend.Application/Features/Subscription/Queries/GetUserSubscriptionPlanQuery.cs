using Agrivision.Backend.Application.Features.Subscription.Contracts;
using Agrivision.Backend.Domain.Abstractions;
using MediatR;

namespace Agrivision.Backend.Application.Features.Subscription.Queries;

public record GetUserSubscriptionPlanQuery
(
    string UserId
) : IRequest<Result<UserSubscriptionPlanResponse>>;