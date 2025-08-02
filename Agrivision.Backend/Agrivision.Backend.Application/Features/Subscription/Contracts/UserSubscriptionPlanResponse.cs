using Agrivision.Backend.Domain.Enums.Core;

namespace Agrivision.Backend.Application.Features.Subscription.Contracts;

public record UserSubscriptionPlanResponse(string PlanName, DateTime? StartDate, DateTime? EndDate, UserSubscriptionStatus Status);