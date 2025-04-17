using Agrivision.Backend.Application.Features.Auth.Contracts;
using FluentValidation;

namespace Agrivision.Backend.Application.Features.Auth.Validators;
public class VerifyOtpRequestValidator : AbstractValidator<VerifyOtpRequest>
{
    public VerifyOtpRequestValidator()
    {
        RuleFor(request => request.Email)
            .NotEmpty()
            .EmailAddress();
        RuleFor(request => request.Otp)
           .NotEmpty();
    }
}