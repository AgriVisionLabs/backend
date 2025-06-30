
using Agrivision.Backend.Application.Auth;
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
                                        IUserContext userContext,
                                        IUserSubscriptionRepository userSubscriptionRepository,
                                        ILogger<ConfirmSubscriptionHandler> logger,
                                        IStripeService stripeService
                                                                 ) : IRequestHandler<ConfirmSubscriptionCommand, Result>
{

    public async Task<Result> Handle(ConfirmSubscriptionCommand request, CancellationToken cancellationToken)
    {
        var userEmail = await stripeService.GetCustomerEmailAsync(request.SessionId);
        var user = await userRepository.FindByEmailAsync(userEmail);
        if (user == null)
        {
            logger.LogWarning("User not found: {userEmail}", userEmail);
            return Result.Failure(UserErrors.UserNotFound);
        }

        var planId = await stripeService.GetPlanIdAsync(request.SessionId);
        
        var plan = await planRepository.GetByIdAsync(planId??Guid.Empty, cancellationToken);
        if (plan == null || !plan.IsActive)
        {
            logger.LogWarning("Invalid or inactive plan: {PlanId}", planId);
            return Result.Failure(SubscriptionPlanErrors.InvalidPlan);
        }

        var existingSubscription = await userSubscriptionRepository.GetByUserIdAsync(user.Id, cancellationToken);
        if (existingSubscription != null && existingSubscription.Status == UserSubscriptionStatus.Active)
        {
            logger.LogWarning("User {UserId} already has an active subscription.", user.Id);
            return Result.Failure(SubscriptionPlanErrors.AleardyActivated);
        }

        try
        {

           

            var stripeSubscriptionId =await stripeService.GetSubscriptionIdAsync(request.SessionId);

            var userSubscription = new UserSubscription
            {
                UserId = user.Id,
                SubscriptionPlanId = plan.Id,
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddMonths(1),
                Status = UserSubscriptionStatus.Active,
                PaymentStatus = PaymentStatus.Paid,
                StripeSubscriptionId = stripeSubscriptionId!,
                CreatedById= user.Id
            };

            await userSubscriptionRepository.AddAsync(userSubscription, cancellationToken);
            logger.LogInformation("User {UserId} subscribed to plan {PlanId} after payment confirmation", user.Id, planId);

            return Result.Success();

        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to confirm subscription for user {UserId} and plan {PlanId}", user.Id, planId);
            return Result.Failure(SubscriptionPlanErrors.FailedToSubscripe);
        }
    }

}