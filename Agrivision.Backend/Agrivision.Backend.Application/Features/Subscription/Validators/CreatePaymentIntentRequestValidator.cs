
using Agrivision.Backend.Application.Features.Subscription.Contracts;
using FluentValidation;

namespace Agrivision.Backend.Application.Features.Subscription.Validators;
public class CreateSubscriptionRequestValidator : AbstractValidator<CreateSubscriptionRequest>
{
    public CreateSubscriptionRequestValidator()
    {
        RuleFor(request => request.PlanId)
            .NotEmpty();
    }
}
