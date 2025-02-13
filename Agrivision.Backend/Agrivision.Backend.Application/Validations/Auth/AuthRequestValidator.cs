using Agrivision.Backend.Application.Contracts.Auth;
using FluentValidation;
using FluentValidation.AspNetCore;

namespace Agrivision.Backend.Application.Validations.Auth;

public class AuthRequestValidator : AbstractValidator<AuthRequest>
{
    public AuthRequestValidator()
    {
        RuleFor(request => request.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(request => request.Password)
            .NotEmpty();
    }
}