
using Agrivision.Backend.Application.Features.Subscription.Contracts;
using FluentValidation;

namespace Agrivision.Backend.Application.Features.Subscription.Validators;
public class CreatePaymentIntentRequestValidator : AbstractValidator<CreatePaymentIntentRequest>
{
    public CreatePaymentIntentRequestValidator()
    {
        RuleFor(request => request.UserId)
            .NotEmpty();
        RuleFor(request => request.PlanId)
            .NotEmpty();
    }
}
