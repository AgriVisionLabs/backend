using Agrivision.Backend.Application.Features.Auth.Contracts;
using FluentValidation;

namespace Agrivision.Backend.Application.Features.Auth.Validators;

public class VerifyPasswordResetRequestValidator : AbstractValidator<VerifyPasswordResetOtpRequest>
{
    public VerifyPasswordResetRequestValidator()
    {
        RuleFor(req => req.Email)
            .EmailAddress()
            .NotEmpty();

        RuleFor(req => req.OtpCode)
            .NotEmpty()
            .Length(6);
    }
}