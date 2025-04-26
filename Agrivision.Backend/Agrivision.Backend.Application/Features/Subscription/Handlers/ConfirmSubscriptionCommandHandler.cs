
using Agrivision.Backend.Application.Errors;
using Agrivision.Backend.Application.Features.Subscription.Commands;
using Agrivision.Backend.Application.Repositories.Core;
using Agrivision.Backend.Application.Repositories.Identity;
using Agrivision.Backend.Application.Services.Payment;
using Agrivision.Backend.Domain.Abstractions;
using Agrivision.Backend.Domain.Entities.Core;
using Agrivision.Backend.Domain.Enums.Core;
using MediatR;
using Microsoft.Extensions.Logging;


namespace Agrivision.Backend.Application.Features.Subscription.Handlers;
public class ConfirmSubscriptionHandler(IUserRepository userRepository,
                                        ISubscriptionPlanRepository planRepository,
                                        IUserSubscriptionRepository userSubscriptionRepository,
                                        ILogger<ConfirmSubscriptionHandler> logger,
                                        IStripeService stripeService
                                                                 ) : IRequestHandler<ConfirmSubscriptionCommand, Result>
{

    public async Task<Result> Handle(ConfirmSubscriptionCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.FindByIdAsync(request.UserId);
        if (user == null)
        {
            logger.LogWarning("User not found: {UserId}", request.UserId);
            return Result.Failure(UserErrors.UserNotFound);
        }

        var plan = await planRepository.GetByIdAsync(request.PlanId, cancellationToken);
        if (plan == null || !plan.IsActive)
        {
            logger.LogWarning("Invalid or inactive plan: {PlanId}", request.PlanId);
            return Result.Failure(SubscriptionPlanErrors.InvalidPlan);
        }

        var existingSubscription = await userSubscriptionRepository.GetByUserIdAsync(request.UserId, cancellationToken);
        if (existingSubscription != null && existingSubscription.Status == UserSubscriptionStatus.Active)
        {
            logger.LogWarning("User {UserId} already has an active subscription.", request.UserId);
            return Result.Failure(SubscriptionPlanErrors.AleardyActivated);
        }

        try
        {
        
            var stripeSubscriptionId = await stripeService.CreateSubscriptionAfterPaymentAsync(request.PaymentIntentId, plan);
            var userSubscription = new UserSubscription
            {
                UserId = request.UserId,
                SubscriptionPlanId = plan.Id,
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddMonths(1),
                Status = UserSubscriptionStatus.Active,
                PaymentStatus = PaymentStatus.Paid,
                StripeSubscriptionId = stripeSubscriptionId
            };

            await userSubscriptionRepository.AddAsync(userSubscription, cancellationToken);
            logger.LogInformation("User {UserId} subscribed to plan {PlanId} after payment confirmation", request.UserId, request.PlanId);

            return Result.Success();

        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to confirm subscription for user {UserId} and plan {PlanId}", request.UserId, request.PlanId);
            return Result.Failure(SubscriptionPlanErrors.FailedToSubscripe);
        }
    }

}