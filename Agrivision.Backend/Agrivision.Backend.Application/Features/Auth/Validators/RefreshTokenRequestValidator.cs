using Agrivision.Backend.Application.Features.Auth.Commands;
using Agrivision.Backend.Application.Features.Auth.Contracts;
using FluentValidation;

namespace Agrivision.Backend.Application.Features.Auth.Validators;

public class RefreshTokenRequestValidator : AbstractValidator<RefreshTokenRequest>
{
    public RefreshTokenRequestValidator()
    {
        RuleFor(req => req.Token)
            .NotEmpty();

        RuleFor(req => req.RefreshToken)
            .NotEmpty();
    }
}