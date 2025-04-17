using Agrivision.Backend.Application.Features.Auth.Contracts;
using Agrivision.Backend.Application.Features.Auth.Queries;
using FluentValidation;

namespace Agrivision.Backend.Application.Features.Auth.Validators;

public class AuthRequestValidator : AbstractValidator<AuthRequest>
{
    public AuthRequestValidator()
    {
        RuleFor(req => req.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(req => req.Password)
            .NotEmpty();
    }
}