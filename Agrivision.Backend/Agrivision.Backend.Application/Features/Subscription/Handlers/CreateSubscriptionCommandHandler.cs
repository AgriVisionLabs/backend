using Agrivision.Backend.Application.Errors;
using Agrivision.Backend.Application.Features.Subscription.Commands;
using Agrivision.Backend.Application.Features.Subscription.Contracts;
using Agrivision.Backend.Application.Repositories.Core;
using Agrivision.Backend.Application.Repositories.Identity;
using Agrivision.Backend.Application.Services.Payment;
using Agrivision.Backend.Domain.Abstractions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Agrivision.Backend.Application.Features.Subscription.Handlers;
public class CreateSubscriptionCommandHandler(
                                               IUserRepository userRepository,
                                               ISubscriptionPlanRepository planRepository,
                                               ILogger<CreateSubscriptionCommandHandler> logger,
                                               IStripeService stripeService
                                                              )
                                                              : IRequestHandler<CreateSubscriptionCommand, Result<CreateSubscriptionResponse>>
{
    public async Task<Result<CreateSubscriptionResponse>> Handle(CreateSubscriptionCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.FindByIdAsync(request.UserId);
        if (user == null)
        {
            logger.LogWarning("User not found: {UserId}", request.UserId);
            return Result.Failure<CreateSubscriptionResponse>(UserErrors.UserNotFound);
        }

        var plan = await planRepository.GetByIdAsync(request.PlanId, cancellationToken);
        if (plan == null || !plan.IsActive)
        {
            logger.LogWarning("Invalid or inactive plan: {PlanId}", request.PlanId);
            return Result.Failure<CreateSubscriptionResponse>(SubscriptionPlanErrors.InvalidPlan);
        }

        if (plan.Price == 0) // Free plan doesn't need payment
            return Result.Failure<CreateSubscriptionResponse>(SubscriptionPlanErrors.FreePlan);

        var sessionUrl = await stripeService.CreateSubscriptionCheckoutSessionAsync(user.Email, plan);
        logger.LogInformation("CheckOut session have created for user {UserId} and plan {PlanId}", request.UserId, request.PlanId);

        var response = new CreateSubscriptionResponse(sessionUrl);
        return Result.Success(response);
    }
}
