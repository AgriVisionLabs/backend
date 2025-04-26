using Agrivision.Backend.Application.Features.Subscription.Contracts;
using FluentValidation;

namespace Agrivision.Backend.Application.Features.Subscription.Validators;
 

public class ConfirmSubscriptionRequestValidator : AbstractValidator<ConfirmSubscriptionRequest>
{
    public ConfirmSubscriptionRequestValidator()
    {
        RuleFor(request => request.UserId)
            .NotEmpty();
        RuleFor(request => request.PlanId)
            .NotEmpty();
        RuleFor(request => request.PaymentIntentId)
            .NotEmpty();
    }
}
