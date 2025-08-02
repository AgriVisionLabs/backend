using Agrivision.Backend.Application.Features.Auth.Contracts;
using FluentValidation;

namespace Agrivision.Backend.Application.Features.Auth.Validators;

public class VerifyMfaOtpRequestValidator : AbstractValidator<VerifyMfaOtpRequest>
{
    public VerifyMfaOtpRequestValidator()
    {
        RuleFor(r => r.Email)
            .EmailAddress()
            .NotEmpty();
        
        RuleFor(r => r.OtpCode)
            .NotEmpty()
            .Length(6);
    }
}