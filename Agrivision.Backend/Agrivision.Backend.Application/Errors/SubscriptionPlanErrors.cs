using Agrivision.Backend.Domain.Abstractions;
namespace Agrivision.Backend.Application.Errors;
public static class SubscriptionPlanErrors
{
    public static readonly Error InvalidPlan =
       new("Plan.Invalid", "Invalid subscription plan.");

    public static readonly Error FreePlan =
      new("Plan.Free", "This plan does not require payment.");

    public static readonly Error AleardyActivated =
     new("Plan.AleardyActivatedSubscribtions", "You already have an active subscription.");

    public static readonly Error FailedToSubscripe =
    new("Plan.FailedToSubscripe", "Failed to confirm subscription. Please try again.");

    public static readonly Error FailedToPay =
    new("Plan.FailedToPay", "the payment prossess is incomplete.");

}
