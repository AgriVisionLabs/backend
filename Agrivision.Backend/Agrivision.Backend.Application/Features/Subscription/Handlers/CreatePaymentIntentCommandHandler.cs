using Agrivision.Backend.Application.Errors;
using Agrivision.Backend.Application.Features.Subscription.Commands;
using Agrivision.Backend.Application.Repositories.Core;
using Agrivision.Backend.Application.Repositories.Identity;
using Agrivision.Backend.Application.Services.Payment;
using Agrivision.Backend.Domain.Abstractions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Agrivision.Backend.Application.Features.Subscription.Handlers;
public class CreatePaymentIntentCommandHandler(
                                               IUserRepository userRepository,
                                               ISubscriptionPlanRepository planRepository,
                                               ILogger<CreatePaymentIntentCommandHandler> logger,
                                               IStripeService stripeService
                                                              )
                                                              : IRequestHandler<CreatePaymentIntentCommand, Result<string>>
{
    public async Task<Result<string>> Handle(CreatePaymentIntentCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.FindByIdAsync(request.UserId);
        if (user == null)
        {
            logger.LogWarning("User not found: {UserId}", request.UserId);
            return Result.Failure<string>(UserErrors.UserNotFound);
        }

        var plan = await planRepository.GetByIdAsync(request.PlanId, cancellationToken);
        if (plan == null || !plan.IsActive)
        {
            logger.LogWarning("Invalid or inactive plan: {PlanId}", request.PlanId);
            return Result.Failure<string>(SubscriptionPlanErrors.InvalidPlan);
        }

        if (plan.Price == 0) // Free plan doesn't need payment
            return Result.Failure<string>(SubscriptionPlanErrors.FreePlan);

        var clientSecret = await stripeService.CreatePaymentIntentAsync(user.Email, plan);
        logger.LogInformation("Payment Intent created for user {UserId} and plan {PlanId}", request.UserId, request.PlanId);

        return Result.Success(clientSecret);
    }
}
