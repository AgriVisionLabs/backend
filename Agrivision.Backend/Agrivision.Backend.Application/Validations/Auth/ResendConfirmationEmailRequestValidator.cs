using Agrivision.Backend.Application.Contracts.Auth;
using FluentValidation;
using FluentValidation.AspNetCore;

namespace Agrivision.Backend.Application.Validations.Auth;

public class ResendConfirmationEmailRequestValidator : AbstractValidator<ResendConfirmationEmailRequest>
{
    public ResendConfirmationEmailRequestValidator()
    {
        RuleFor(request => request.Email)
            .NotEmpty()
            .EmailAddress();
    }
}