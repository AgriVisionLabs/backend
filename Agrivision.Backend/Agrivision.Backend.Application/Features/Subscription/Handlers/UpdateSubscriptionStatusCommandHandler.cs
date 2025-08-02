using Agrivision.Backend.Application.Errors;
using Agrivision.Backend.Application.Features.Subscription.Commands;
using Agrivision.Backend.Application.Repositories.Core;
using Agrivision.Backend.Domain.Abstractions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Agrivision.Backend.Application.Features.Subscription.Handlers;
public class UpdateSubscriptionStatusCommandHandler(IUserSubscriptionRepository userSubscriptionRepository, ILogger<ConfirmSubscriptionHandler> logger) : IRequestHandler<UpdateSubscriptionStatusCommand, Result>
{
    public async Task<Result> Handle(UpdateSubscriptionStatusCommand request, CancellationToken cancellationToken)
    {
        var subscription = await userSubscriptionRepository.GetByStripeSubscriptionIdAsync(request.StripeSubscriptionId, cancellationToken);
        if (subscription == null)
        {
            return Result.Failure(SubscriptionPlanErrors.SubscriptionNotFound);
        }

        subscription.Status = request.Status;
        subscription.UpdatedOn = DateTime.UtcNow;

        await userSubscriptionRepository.UpdateAsync(subscription, cancellationToken);

        return Result.Success();
    }
}
