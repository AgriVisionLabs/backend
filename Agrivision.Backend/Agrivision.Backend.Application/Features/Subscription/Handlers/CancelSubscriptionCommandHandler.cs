using Agrivision.Backend.Application.Errors;
using Agrivision.Backend.Application.Features.Subscription.Commands;
using Agrivision.Backend.Application.Repositories.Core;
using Agrivision.Backend.Domain.Abstractions;
using Agrivision.Backend.Domain.Enums.Core;
using MediatR;

namespace Agrivision.Backend.Application.Features.Subscription.Handlers;

public class CancelSubscriptionCommandHandler(IUserSubscriptionRepository userSubscriptionRepository) : IRequestHandler<CancelSubscriptionCommand, Result>
{
    public async Task<Result> Handle(CancelSubscriptionCommand request, CancellationToken cancellationToken)
    {
        var subscription = await userSubscriptionRepository.GetByUserIdAsync(request.UserId, cancellationToken);
        if (subscription is null)
            return Result.Failure(UserErrors.CannotCancelBasicPlan);

        subscription.Status = UserSubscriptionStatus.Canceled;
        subscription.UpdatedById = request.UserId;
        subscription.UpdatedOn = DateTime.UtcNow;
        
        await userSubscriptionRepository.UpdateAsync(subscription, cancellationToken);
        
        return Result.Success();
    }
}
