using Agrivision.Backend.Application.Features.Subscription.Contracts;
using Agrivision.Backend.Application.Features.Subscription.Queries;
using Agrivision.Backend.Application.Repositories.Core;
using Agrivision.Backend.Domain.Abstractions;
using Agrivision.Backend.Domain.Enums.Core;
using MediatR;

namespace Agrivision.Backend.Application.Features.Subscription.Handlers;

public class GetUserSubscriptionQueryHandler(IUserSubscriptionRepository userSubscriptionRepository) : IRequestHandler<GetUserSubscriptionPlanQuery, Result<UserSubscriptionPlanResponse>>
{
    public async Task<Result<UserSubscriptionPlanResponse>> Handle(GetUserSubscriptionPlanQuery request, CancellationToken cancellationToken)
    {
        var userSubscription = await userSubscriptionRepository.GetByUserIdAsync(request.UserId, cancellationToken);
        if (userSubscription is null)
            return Result.Success(new UserSubscriptionPlanResponse("Basic", null, null, UserSubscriptionStatus.Active));
        
        return Result.Success(new UserSubscriptionPlanResponse(
            userSubscription.SubscriptionPlan.Name,
            userSubscription.StartDate,
            userSubscription.EndDate,
            userSubscription.Status
        ));
    }
}